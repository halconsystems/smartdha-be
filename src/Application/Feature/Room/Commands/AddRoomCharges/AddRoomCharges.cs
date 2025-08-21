using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomCharges.Dtos;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomCharges;
public record AddRoomCharges(Guid RoomId, RoomChargesDto Charge) : IRequest<Guid>;
public class AddRoomChargesHandler : IRequestHandler<AddRoomCharges, Guid>
{
    private readonly IOLMRSApplicationDbContext _context;

    public AddRoomChargesHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddRoomCharges request, CancellationToken cancellationToken)
    {
        var roomCharge = new RoomCharge
        {
            RoomId = request.RoomId,
            BookingType = request.Charge.BookingType,
            Charges = request.Charge.Charges
        };

        _context.RoomCharges.Add(roomCharge);
        await _context.SaveChangesAsync(cancellationToken);

        return roomCharge.Id;
    }
}
