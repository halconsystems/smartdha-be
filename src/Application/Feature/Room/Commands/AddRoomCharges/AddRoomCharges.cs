using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomCharges.Dtos;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.AddRoomCharges;
public record AddRoomCharges(RoomChargesDto Charge) : IRequest<SuccessResponse<Guid>>;
public class AddRoomChargesHandler : IRequestHandler<AddRoomCharges, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public AddRoomChargesHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(AddRoomCharges request, CancellationToken cancellationToken)
    {
        var roomCharge = new RoomCharge
        {
            RoomId = request.Charge.RoomId,
            BookingType = request.Charge.BookingType,
            NoOfOccupancy = request.Charge.NoOfOccupancy,
            Charges = request.Charge.Charges
        };

        _context.RoomCharges.Add(roomCharge);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<Guid>(roomCharge.Id, "Room Charges Added.");
        //return roomCharge.Id;
    }
}
