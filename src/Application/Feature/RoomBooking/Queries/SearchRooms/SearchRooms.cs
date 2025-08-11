using MediatR;
using Microsoft.EntityFrameworkCore;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;

public record SearchRoomsQuery(
    Guid ClubId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    string BookingType // "self" or "guest"
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

    public async Task<List<SearchRoomsDto>> Handle(SearchRoomsQuery request, CancellationToken cancellationToken)
    {
        var availableRooms = await (
            from r in _context.Rooms
            join ra in _context.RoomsAvailability
                on r.Id equals ra.RoomId
            where r.ClubId == request.ClubId
                && r.IsGloballyAvailable
                && ra.FromDate <= request.CheckInDate
                && ra.ToDate >= request.CheckOutDate
                && ra.Action == AvailabilityAction.Available
            select new SearchRoomsDto
            {
                ResidenceTypeName = _context.ResidenceTypes
                    .Where(rt => rt.Id == r.ResidenceTypeId)
                    .Select(rt => rt.Name)
                    .FirstOrDefault() ?? string.Empty,

                CategoryName = _context.RoomCategories
                    .Where(rc => rc.Id == r.RoomCategoryId)
                    .Select(rc => rc.Name)
                    .FirstOrDefault() ?? string.Empty,

                RoomId = r.Id,
                Name = r.Name ?? string.Empty,
                RoomNo = r.No,

                Price = _context.RoomCharges
                    .Where(rc => rc.RoomId == r.Id && rc.BookingType == request.BookingType)
                    .Select(rc => rc.Charges)
                    .FirstOrDefault(),

                Ratings = _context.RoomRatings
                    .Where(rr => rr.RoomId == r.Id)
                    .Select(rr => rr.RoomRating)
                    .FirstOrDefault(),

                DefaultImage = _context.RoomImages
                    .Where(img => img.RoomId == r.Id && img.Category == ImageCategory.Main)
                    .Select(img => img.ImageURL)
                    .FirstOrDefault() ?? string.Empty,

                CheckInDate = ra.FromDate,
                CheckOutDate = ra.ToDate
            })
            .ToListAsync(cancellationToken); // async version

        // Now safely modify in memory
        availableRooms.ForEach(r =>
        {
            r.DefaultImage = string.IsNullOrEmpty(r.DefaultImage)
                ? string.Empty
                : _fileStorageService.GetPublicUrl(r.DefaultImage);
        });

        return availableRooms;
    }
}
