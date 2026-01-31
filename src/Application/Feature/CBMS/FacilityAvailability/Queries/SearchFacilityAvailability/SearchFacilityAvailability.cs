using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityAvailability.Queries.SearchFacilityAvailability;
public record SearchFacilityAvailabilityQuery(
    Guid ClubId,
    Guid? FacilityId,
    DateOnly? Date,
    DateOnly? FromDate,
    DateOnly? ToDate
) : IRequest<ApiResult<List<FacilitySearchResponse>>>;
public class SearchFacilityAvailabilityHandler
    : IRequestHandler<SearchFacilityAvailabilityQuery, ApiResult<List<FacilitySearchResponse>>>
{
    private readonly ICBMSApplicationDbContext _db;

    public SearchFacilityAvailabilityHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<FacilitySearchResponse>>> Handle(
        SearchFacilityAvailabilityQuery request,
        CancellationToken ct)
    {
        var facilitiesQuery =
            from f in _db.Facilities
            join cf in _db.ClubFacilities on f.Id equals cf.FacilityId
            where cf.ClubId == request.ClubId
            select f;

        if (request.FacilityId.HasValue)
            facilitiesQuery = facilitiesQuery.Where(x => x.Id == request.FacilityId);

        var facilities = await facilitiesQuery
            .Include(f => f.ClubCategory)
            .ToListAsync(ct);

        var result = new List<FacilitySearchResponse>();

        foreach (var facility in facilities)
        {
            var units = await _db.FacilityUnits
                .Where(u => u.ClubId == request.ClubId && u.FacilityId == facility.Id && u.IsActive ==true)
                .ToListAsync(ct);

            var unitResponses = new List<FacilityUnitAvailabilityDto>();

            foreach (var unit in units)
            {
                var config = await _db.FacilityUnitBookingConfigs
                    .FirstAsync(x => x.FacilityUnitId == unit.Id, ct);

                if (config.BookingMode == BookingMode.SlotBased)
                {
                    // 🟢 PADEL COURT LOGIC
                    if (!config.OpeningTime.HasValue || !config.ClosingTime.HasValue)
                    {
                        throw new InvalidOperationException(
                            "OpeningTime and ClosingTime must be configured for slot-based facilities.");
                    }

                    if (!config.SlotDurationMinutes.HasValue || config.SlotDurationMinutes <= 0)
                    {
                        throw new InvalidOperationException(
                            "SlotDurationMinutes must be configured for slot-based facilities.");
                    }

                    var slots = GenerateSlots(
                        config.OpeningTime.Value,
                        config.ClosingTime.Value,
                        config.SlotDurationMinutes.Value
                    );


                    var bookings = await _db.BookingSchedules
                        .Where(b =>
                            b.Date == request.Date &&
                            b.Booking.FacilityUnitId == unit.Id &&
                            b.Booking.Status != BookingStatus.Cancelled)
                        .ToListAsync(ct);

                    var slotDtos = slots.Select(s => new SlotAvailabilityDto(
                        s.start,
                        s.end,
                        config.BasePrice,
                        !bookings.Any(b =>
                            s.start < b.EndTime &&
                            s.end > b.StartTime)
                    )).ToList();

                    unitResponses.Add(new FacilityUnitAvailabilityDto(
                        unit.Id,
                        unit.Name,
                        unit.UnitType,
                        config.BasePrice,
                        slotDtos.Any(x => x.IsAvailable),
                        slotDtos
                    ));
                }
                else
                {
                    // 🟡 BANQUET / GUEST ROOM
                    var hasConflict = await _db.BookingDateRanges.AnyAsync(b =>
                     b.Booking.FacilityUnitId == unit.Id &&
                     b.Booking.Status != BookingStatus.Cancelled &&
                     request.FromDate < b.ToDate &&
                     request.ToDate > b.FromDate,
                     ct);


                    unitResponses.Add(new FacilityUnitAvailabilityDto(
                        unit.Id,
                        unit.Name,
                        unit.UnitType,
                        config.BasePrice,
                        !hasConflict,
                        null
                    ));
                }
            }

            result.Add(new FacilitySearchResponse(
                facility.Id,
                facility.DisplayName,
                facility.ClubCategory.Name,
                _db.FacilitiesImages
                    .Where(x => x.FacilityId == facility.Id && x.Category==ImageCategory.Main)
                    .Select(x => x.ImageURL)
                    .FirstOrDefault(),
                unitResponses.First().Slots != null
                    ? BookingMode.SlotBased
                    : BookingMode.DayBased,
                unitResponses
            ));
        }

        return ApiResult<List<FacilitySearchResponse>>.Ok(result);
    }

    private static List<(TimeOnly start, TimeOnly end)> GenerateSlots(
    TimeOnly opening,
    TimeOnly closing,
    int minutes)
    {
        var list = new List<(TimeOnly, TimeOnly)>();
        var current = opening;

        while (current.AddMinutes(minutes) <= closing)
        {
            var end = current.AddMinutes(minutes);
            list.Add((current, end));
            current = end;
        }

        return list;
    }

}

