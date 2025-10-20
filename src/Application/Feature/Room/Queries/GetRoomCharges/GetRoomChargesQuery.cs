using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;
using DHAFacilitationAPIs.Application.Feature.RoomCharges.Dtos;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomCharges;
public class GetRoomChargesQuery : IRequest<List<RoomChargesDto>>
{
    public Guid RoomId { get; set; }
    public RoomBookingType? BookingType { get; set; }  // optional filter
}
public class GetRoomChargesQueryHandler : IRequestHandler<GetRoomChargesQuery, List<RoomChargesDto>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetRoomChargesQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoomChargesDto>> Handle(GetRoomChargesQuery request, CancellationToken ct)
    {
        var query = _context.RoomCharges
            .AsNoTracking()
            .Where(rc => rc.RoomId == request.RoomId && (rc.IsDeleted == false && rc.IsActive == true));

        if (request.BookingType.HasValue)
            query = query.Where(rc => rc.BookingType == request.BookingType.Value);

        var groupedCharges = await query
            .GroupBy(rc => new { rc.RoomId, rc.BookingType })
            .Select(g => new RoomChargesDto
            {
                RoomId = g.Key.RoomId,
                BookingType = g.Key.BookingType,
                Charges = g
                    .OrderBy(rc => rc.ExtraOccupancy)
                    .Select(rc => new RoomChargeItemDto
                    {
                        ExtraOccupancy = rc.ExtraOccupancy,
                        Charges = rc.Charges
                    }).ToList()
            })
            .OrderBy(r => r.BookingType)
            .ToListAsync(ct);

        return groupedCharges;
    }
}
