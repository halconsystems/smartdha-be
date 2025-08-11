using MediatR;
using Microsoft.EntityFrameworkCore;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;

public record SearchRoomsQuery(
    Guid ClubId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    string BookingType
) : IRequest<List<SearchRoomsDto>>;

public class SearchRoomsQueryHandler : IRequestHandler<SearchRoomsQuery, List<SearchRoomsDto>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public SearchRoomsQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SearchRoomsDto>> Handle(SearchRoomsQuery request, CancellationToken cancellationToken)
    {
        var availableRooms = await _context.Rooms
            .Where(r => r.ClubId == request.ClubId &&
                        !_context.RoomBookings.Any(rb =>
                            rb.RoomId == r.Id &&
                            rb.CheckInDate < request.CheckOutDate &&
                            rb.CheckOutDate > request.CheckInDate))
            .GroupBy(r => new { r.RoomCategoryId, r.ResidenceTypeId })
            .Select(g => new
            {
                g.Key.RoomCategoryId,
                CategoryName = _context.RoomCategories
                    .Where(rc => rc.Id == g.Key.RoomCategoryId)
                    .Select(rc => rc.Name)
                    .FirstOrDefault(),
                g.Key.ResidenceTypeId,
                ResidenceTypeName = _context.ResidenceTypes
                    .Where(rt => rt.Id == g.Key.ResidenceTypeId)
                    .Select(rt => rt.Name)
                    .FirstOrDefault(),
                Rooms = g.Select(r => new RoomDto
                {
                    RoomId = r.Id,
                    Name = r.Name ?? "",
                    RoomNo = r.No,
                    Price = _context.RoomCharges
                        .Where(rc => rc.RoomId == r.Id && rc.BookingType == request.BookingType)
                        .Select(rc => rc.Charges)
                        .FirstOrDefault(),
                    Ratings = _context.RoomRatings
                        .Where(rate => rate.RoomId == r.Id)
                        .Select(rate => rate.RoomRating)
                        .FirstOrDefault(),
                    Images = _context.RoomImages
                        .Where(img => img.RoomId == r.Id)
                        .Select(img => img.ImageURL)
                        .ToList()
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return availableRooms
            .GroupBy(x => new { x.RoomCategoryId, x.CategoryName })
            .Select(categoryGroup => new SearchRoomsDto
            {
                CategoryId = categoryGroup.Key.RoomCategoryId,
                CategoryName = categoryGroup.Key.CategoryName,
                ResidenceTypes = categoryGroup
                    .GroupBy(rt => new { rt.ResidenceTypeId, rt.ResidenceTypeName })
                    .Select(rtGroup => new ResidenceTypeDto
                    {
                        ResidenceTypeId = rtGroup.Key.ResidenceTypeId,
                        ResidenceTypeName = rtGroup.Key.ResidenceTypeName,
                        Rooms = rtGroup.SelectMany(rt => rt.Rooms).ToList()
                    }).ToList()
            }).ToList();
    }
}

