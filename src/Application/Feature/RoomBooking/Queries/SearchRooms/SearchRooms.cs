using MediatR;
using Microsoft.EntityFrameworkCore;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;

public record SearchRoomsQuery(
    Guid ClubId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    RoomBookingType BookingType // "self" or "guest"
) : IRequest<List<SearchRoomsDto>>;

public class SearchRoomsQueryHandler : IRequestHandler<SearchRoomsQuery, List<SearchRoomsDto>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public SearchRoomsQueryHandler(IOLMRSApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<SearchRoomsDto>> Handle(SearchRoomsQuery request, CancellationToken ct)
    {
        var start = request.CheckInDate;   // DateOnly
        var end = request.CheckOutDate;  // DateOnly

        var query =
            from r in _context.Rooms.AsNoTracking()
            where r.ClubId == request.ClubId
               && r.IsGloballyAvailable

            // Check rooms according to their availibility (subset allowed)
            from a in r.Availabilities
                .Where(a => a.Action == AvailabilityAction.Available
                     && a.FromDateOnly <= end   // starts before (or on) the search end
                     && a.ToDateOnly >= start) // ends after (or on) the search start
            where !_context.ReservationRooms.Any(res =>     // Exclude rooms already reserved/booked in overlapping period
                res.RoomId == r.Id &&
                res.FromDateOnly <= end &&
                res.ToDateOnly >= start &&
                (res.Reservation.Status == Domain.Enums.ReservationStatus.AwaitingPayment
                 || res.Reservation.Status == Domain.Enums.ReservationStatus.Converted))
            && !_context.RoomBookings.Any(bk =>
                bk.RoomId == r.Id &&
                bk.Status == BookingStatus.Confirmed &&
                bk.CheckInDateOnly <= end &&
                bk.CheckOutDateOnly >= start)

            select new SearchRoomsDto
            {
                ResidenceTypeName = r.ResidenceType != null ? r.ResidenceType.Name : string.Empty,
                CategoryName = r.RoomCategory != null ? r.RoomCategory.Name : string.Empty,

                RoomId = r.Id,
                Name = r.Name ?? string.Empty,
                RoomNo = r.No,

                // Charges (requested according to BookingType & 1 Occupant)
                Price = _context.RoomCharges
                            .AsNoTracking()
                            .Where(c => c.RoomId == r.Id && c.BookingType == request.BookingType)
                            .Select(c => (decimal?)c.Charges)
                            .FirstOrDefault(),

                
                Ratings = (decimal?)_context.RoomRatings
                            .AsNoTracking()
                            .Where(rr => rr.RoomId == r.Id)
                            .OrderByDescending(rr => rr.Created) // If not Created then remove/set Id
                            .Select(rr => (double?)rr.RoomRatings)
                            .FirstOrDefault(),

              // Main Image
                DefaultImage = _context.RoomImages
                            .AsNoTracking()
                            .Where(img => img.RoomId == r.Id && img.Category == ImageCategory.Main)
                            .OrderBy(img => img.Id)
                            .Select(img => img.ImageURL)
                            .FirstOrDefault() ?? string.Empty,

                // User requested dates
                CheckInDate = start,
                CheckInTimeOnly=a.FromTimeOnly,
                CheckOutDate = end,
                CheckOutTimeOnly=a.ToTimeOnly,

                // Range of this specific available slot

                AvailabilityFrom = a.FromDate,

                AvailabilityTo = a.ToDate
            };

        var list = await query.ToListAsync(ct);

        // Make the image URL public
        list.ForEach(x =>
        {
            x.DefaultImage = string.IsNullOrWhiteSpace(x.DefaultImage)
                ? string.Empty
                : _fileStorageService.GetPublicUrl(x.DefaultImage);
        });

        return list;
    }




   
}
