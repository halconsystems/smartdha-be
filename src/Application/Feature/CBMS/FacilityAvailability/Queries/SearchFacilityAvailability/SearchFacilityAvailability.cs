using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityAvailability.Queries.SearchFacilityAvailability;
public record SearchFacilityAvailabilityQuery(
    Guid ClubId,
    Guid? FacilityId,
    DateOnly? FromDate,
    DateOnly? ToDate
) : IRequest<ApiResult<List<FacilitySearchResponse>>>;
public class SearchFacilityAvailabilityHandler
    : IRequestHandler<SearchFacilityAvailabilityQuery, ApiResult<List<FacilitySearchResponse>>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorageService;

    public SearchFacilityAvailabilityHandler(ICBMSApplicationDbContext db, IFileStorageService fileStorageService)
    {
        _db = db;
        _fileStorageService = fileStorageService;
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
                .Where(u =>
                    u.ClubId == request.ClubId &&
                    u.FacilityId == facility.Id &&
                    u.IsActive==true && u.IsDeleted !=true)
                .ToListAsync(ct);



            var unitResponses = new List<FacilityUnitAvailabilityDto>();

            foreach (var unit in units)
            {
                var unitServices = await _db.FacilityUnitServices
                 .Where(s =>
                     s.FacilityUnitId == unit.Id &&
                     s.IsEnabled &&
                     s.IsDeleted != true)
                 .Select(s => new UnitServiceDto(
                     s.ServiceDefinitionId,
                     s.ServiceDefinition.Name,
                     s.Price,
                     s.IsComplimentary,
                     s.ServiceDefinition.IsQuantityBased
                 ))
                 .ToListAsync(ct);



                var unitMainImagePath = await _db.FacilityUnitImages
                .Where(x =>
                    x.FacilityUnitId == unit.Id &&
                    x.Category == ImageCategory.Main &&
                    x.IsActive == true &&
                    x.IsDeleted != true)
                .Select(x => x.ImageURL)
                .FirstOrDefaultAsync(ct);

                var unitMainImageUrl = unitMainImagePath != null
                    ? _fileStorageService.GetPublicUrl(unitMainImagePath)
                    : null;

                var config = await _db.FacilityUnitBookingConfigs
                    .FirstOrDefaultAsync(x => x.FacilityUnitId == unit.Id, ct);



                if (config == null)
                    continue;

                // Load availability rules (once per unit)
                var rules = await _db.FacilityUnitAvailabilityRules
                    .Where(r => r.FacilityUnitId == unit.Id)
                    .ToListAsync(ct);

                // =========================
                // SLOT BASED (PADEL)
                // =========================
                if (config.BookingMode == BookingMode.SlotBased)
                {
                    if (!request.FromDate.HasValue)
                        continue;

                    // Hard validation
                    if (!config.OpeningTime.HasValue ||
                        !config.ClosingTime.HasValue ||
                        !config.SlotDurationMinutes.HasValue ||
                        config.SlotDurationMinutes <= 0)
                        continue;

                    var slots = GenerateSlots(
                        config.OpeningTime.Value,
                        config.ClosingTime.Value,
                        config.SlotDurationMinutes.Value
                    );

                    var bookings = await (
                     from bs in _db.BookingSchedules
                     join b in _db.Bookings on bs.BookingId equals b.Id
                     where
                         bs.Date == request.FromDate.Value &&
                         b.FacilityUnitId == unit.Id &&
                         b.Status != BookingStatus.Cancelled
                     select bs).ToListAsync(ct);
                
                    var availableSlots = slots
                        .Where(s =>
                            IsSlotAllowed(
                                request.FromDate.Value,
                                s.start,
                                s.end,
                                rules,
                                config.UseAvailabilityRules))
                        .Where(s =>
                            !bookings.Any(b =>
                                s.start < b.EndTime &&
                                s.end > b.StartTime))
                        .Select(s => new SlotAvailabilityDto(
                            s.start,
                            s.end,
                            config.BasePrice,
                            true))
                        .ToList();

                    if (!availableSlots.Any())
                        continue;

                    unitResponses.Add(new FacilityUnitAvailabilityDto(
                        unit.Id,
                        unit.Name,
                        unit.UnitType,
                         unit.Description,
                        config.BasePrice,
                        true,
                        availableSlots,
                        unitMainImageUrl,
                        unitServices
                    ));
                }
                // =========================
                // DAY / NIGHT BASED
                // =========================
                else
                {
                    if (!request.FromDate.HasValue || !request.ToDate.HasValue)
                        continue;

                    var isAllowed = IsDateAllowed(
                        request.FromDate.Value,
                        rules,
                        config.UseAvailabilityRules);

                    if (!isAllowed)
                        continue;

                    var hasConflict = await (
                     from br in _db.BookingDateRanges
                     join b in _db.Bookings on br.BookingId equals b.Id
                     where
                         b.FacilityUnitId == unit.Id &&
                         b.Status != BookingStatus.Cancelled &&
                         request.FromDate < br.ToDate &&
                         request.ToDate > br.FromDate
                     select br.Id
                   ).AnyAsync(ct);


                    if (hasConflict)
                        continue;

                    unitResponses.Add(new FacilityUnitAvailabilityDto(
                        unit.Id,
                        unit.Name,
                        unit.UnitType,
                        unit.Description,
                        config.BasePrice,
                        true,
                        null,
                        unitMainImageUrl,
                        unitServices
                    ));
                }
            }

            if (!unitResponses.Any())
                continue;

            result.Add(new FacilitySearchResponse(
                facility.Id,
                facility.DisplayName,
                facility.ClubCategory.Name,
                _db.FacilitiesImages
                    .Where(x =>
                        x.FacilityId == facility.Id &&
                        x.Category == ImageCategory.Main)
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

    // =========================
    // SLOT GENERATION (SAFE)
    // =========================
    private static List<(TimeOnly start, TimeOnly end)> GenerateSlots(
        TimeOnly opening,
        TimeOnly closing,
        int minutes)
    {
        var list = new List<(TimeOnly, TimeOnly)>();

        var openingMinutes = opening.Hour * 60 + opening.Minute;
        var closingMinutes = closing.Hour * 60 + closing.Minute;

        if (openingMinutes >= closingMinutes)
            return list;

        var current = openingMinutes;

        while (current + minutes <= closingMinutes)
        {
            var start = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(current));
            var end = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(current + minutes));
            list.Add((start, end));
            current += minutes;
        }

        return list;
    }

    // =========================
    // RULE CHECKS
    // =========================
    private static bool IsSlotAllowed(
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        List<FacilityUnitAvailabilityRule> rules,
        bool useRules)
    {
        if (!useRules)
            return true;

        var rule = rules.FirstOrDefault(r =>
            r.Date == date &&
            r.StartTime <= start &&
            r.EndTime >= end);

        return rule?.IsAvailable ?? false;
    }

    private static bool IsDateAllowed(
        DateOnly date,
        List<FacilityUnitAvailabilityRule> rules,
        bool useRules)
    {
        if (!useRules)
            return true;

        var rule = rules.FirstOrDefault(r => r.Date == date);
        return rule?.IsAvailable ?? false;
    }
}
