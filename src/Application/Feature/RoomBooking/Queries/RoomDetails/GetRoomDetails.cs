using DHAFacilitationAPIs.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.RoomDetails;

public record GetRoomDetailsQuery(Guid RoomId, string BookingType) : IRequest<RoomDetailsDto>;

public class GetRoomDetailsQueryHandler : IRequestHandler<GetRoomDetailsQuery, RoomDetailsDto>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetRoomDetailsQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoomDetailsDto> Handle(GetRoomDetailsQuery request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
            .Where(r => r.Id == request.RoomId)
            .Select(r => new RoomDetailsDto
            {
                Name = r.Name,
                No = r.No,
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
                    .ToList(),
                Services = (
                    from mapping in _context.ServiceMappings
                    join service in _context.Services
                        on mapping.ServiceId equals service.Id
                    where mapping.RoomId == r.Id
                    select service.Name
                ).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return room ?? new RoomDetailsDto();
    }
}
