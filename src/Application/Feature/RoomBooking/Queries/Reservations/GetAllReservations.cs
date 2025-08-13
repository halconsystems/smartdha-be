using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
public record GetAllReservationsQuery(Guid UserId) : IRequest<List<ReservationListDto>>;

public class GetAllReservationsQueryHandler : IRequestHandler<GetAllReservationsQuery, List<ReservationListDto>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetAllReservationsQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReservationListDto>> Handle(GetAllReservationsQuery request, CancellationToken cancellationToken)
    {
        var reservations = await _context.Reservations
            .Where(r => r.UserId == request.UserId)
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                    .ThenInclude(room => room.ResidenceType)
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                    .ThenInclude(room => room.RoomCategory)
            .Include(r => r.Club)
            .ToListAsync(cancellationToken);

        var result = reservations
            .Select(r => new ReservationListDto
            {
                ReservationId = r.Id,
                TotalAmount = r.TotalAmount,
                Rooms = r.ReservationRooms.Select(rr => new ReservationRoomListDto
                {
                    RoomNo = rr.Room?.No ?? string.Empty,
                    ClubName = r.Club?.Name ?? string.Empty,
                    ResidenceType = rr.Room?.ResidenceType?.Name ?? string.Empty,
                    RoomCategory = rr.Room?.RoomCategory?.Name ?? string.Empty,
                    FromDate = rr.FromDate,
                    ToDate = rr.ToDate
                }).ToList()
            })
            .ToList();

        return result;
    }
}
