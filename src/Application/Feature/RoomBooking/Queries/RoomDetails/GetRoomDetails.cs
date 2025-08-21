using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.RoomDetails;

public record GetRoomDetailsQuery(Guid RoomId, RoomBookingType BookingType)
    : IRequest<RoomDetailsDto>;

public class GetRoomDetailsQueryHandler : IRequestHandler<GetRoomDetailsQuery, RoomDetailsDto>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public GetRoomDetailsQueryHandler(IOLMRSApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<RoomDetailsDto> Handle(GetRoomDetailsQuery request, CancellationToken cancellationToken)
    {
        var roomData = await _context.Rooms
            .Where(r => r.Id == request.RoomId)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.No,
                r.Description,
                Price = _context.RoomCharges
                    .Where(rc => rc.RoomId == r.Id && rc.BookingType == request.BookingType)
                    .Select(rc => rc.Charges)
                    .FirstOrDefault(),
                Ratings = _context.RoomRatings
                    .Where(rr => rr.RoomId == r.Id)
                    .Select(rr => rr.RoomRatings)
                    .FirstOrDefault(),
                Services = (from m in _context.ServiceMappings
                            join s in _context.Services on m.ServiceId equals s.Id
                            where m.RoomId == r.Id
                            select s.Name).ToList(),
                ResidenceTypeName = r.ResidenceType != null && r.ResidenceType.Name != null
    ? r.ResidenceType.Name
    : _context.ResidenceTypes
        .Where(rt => rt.Id == r.ResidenceTypeId)
        .Select(rt => rt.Name)
        .FirstOrDefault() ?? string.Empty,

                CategoryName = r.RoomCategory != null && r.RoomCategory.Name != null
    ? r.RoomCategory.Name
    : _context.RoomCategories
        .Where(rc => rc.Id == r.RoomCategoryId)
        .Select(rc => rc.Name)
        .FirstOrDefault() ?? string.Empty,

                // Just pick the first matching availability range
                FromDate = _context.RoomAvailabilities
                    .Where(a => a.RoomId == r.Id)
                    .OrderBy(a => a.FromDate)
                    .Select(a => a.FromDate)
                    .FirstOrDefault(),
                ToDate = _context.RoomAvailabilities
                    .Where(a => a.RoomId == r.Id)
                    .OrderBy(a => a.FromDate)
                    .Select(a => a.ToDate)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (roomData == null)
            return new RoomDetailsDto();

        // Get raw image paths first (from DB)
        var imagePaths = await _context.RoomImages
            .Where(img => img.RoomId == roomData.Id)
            .Select(img => img.ImageURL)
            .ToListAsync(cancellationToken);

        // Now convert to public URLs in memory
        var imageUrls = imagePaths
            .Select(path => _fileStorageService.GetPublicUrl(path))
            .ToList();

        return new RoomDetailsDto
        {
            RoomId = roomData.Id,
            Name = roomData.Name,
            No = roomData.No,
            Price = roomData.Price,
            Ratings = roomData.Ratings,
            Images = imageUrls,
            Services = roomData.Services,
            CheckInDate = roomData.FromDate,
            CheckOutDate = roomData.ToDate,
            ResidenceTypeName=roomData.ResidenceTypeName,
            CategoryName=roomData.CategoryName,
            Description=roomData.Description
        };
    }
}

