//using DHAFacilitationAPIs.Application.Common.Interfaces;
//using DHAFacilitationAPIs.Domain.Enums;
//using MediatR;
//using Microsoft.EntityFrameworkCore;

//namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;

//public record SearchRoomsQuery(
//    Guid ClubId,
//    DateTime CheckInDate,
//    DateTime CheckOutDate,
//    string BookingType
//) : IRequest<List<SearchRoomsDto>>;

//public class SearchRoomsQueryHandler : IRequestHandler<SearchRoomsQuery, List<SearchRoomsDto>>
//{
//    private readonly IOLMRSApplicationDbContext _context;

//    public SearchRoomsQueryHandler(IOLMRSApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<List<SearchRoomsDto>> Handle(SearchRoomsQuery request, CancellationToken cancellationToken)
//    {
//        var availableRooms = await _context.RoomAvailability
//    .Where(a =>
//        a.Action == AvailabilityAction.Available &&
//        a.FromDate <= checkIn &&
//        a.ToDate >= checkOut)
//    .Select(a => new
//    {
//        a.Room,
//        a.FromDate,
//        a.ToDate
//    })
//    .ToListAsync(ct);

//        var result = availableRooms.Select(av => new SearchRoomResponseDto
//        {
//            ResidenceTypeName = av.Room.ResidenceType.Name,
//            CategoryName = av.Room.Category.Name,
//            RoomId = av.Room.Id,
//            Name = av.Room.Name,
//            RoomNo = av.Room.RoomNo,
//            Price = av.Room.Price,
//            Ratings = av.Room.Ratings,
//            CheckInDate = av.FromDate,
//            CheckOutDate = av.ToDate,
//            DefaultImage = _fileStorageService.GetPublicUrl(
//                av.Room.Images
//                    .Where(img => img.Category == ImageCategory.Main)
//                    .Select(img => img.ImageURL)
//                    .FirstOrDefault() ?? string.Empty
//            )
//        }).ToList();


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
        var availableRooms = await _context.Rooms
            .Where(r => r.ClubId == request.ClubId &&
                        r.IsGloballyAvailable &&
                        _context.RoomsAvailability.Any(ra =>
                            ra.RoomId == r.Id &&
                            ra.FromDate <= request.CheckInDate &&
                            ra.ToDate >= request.CheckOutDate &&
                            ra.Action == AvailabilityAction.Available
                        )
            )
            .Select(r => new SearchRoomsDto
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

                CheckInDate = _context.RoomsAvailability
                    .Where(a => a.RoomId == r.Id && a.Action == AvailabilityAction.Available)
                    .Select(a => a.FromDate)
                    .FirstOrDefault(),

                CheckOutDate = _context.RoomsAvailability
                    .Where(a => a.RoomId == r.Id && a.Action == AvailabilityAction.Available)
                    .Select(a => a.ToDate)
                    .FirstOrDefault()
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
