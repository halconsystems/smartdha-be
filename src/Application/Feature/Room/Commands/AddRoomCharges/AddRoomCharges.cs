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
public record AddRoomCharges(RoomChargesDto Charge) : IRequest<SuccessResponse<List<Guid>>>;
public class AddRoomChargesHandler : IRequestHandler<AddRoomCharges, SuccessResponse<List<Guid>>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public AddRoomChargesHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<Guid>>> Handle(AddRoomCharges request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
        .Where(r => r.Id == request.Charge.RoomId)
        .FirstOrDefaultAsync(cancellationToken);

        if (room == null)
            throw new Exception($"Room with ID {request.Charge.RoomId} not found.");

        var addedIds = new List<Guid>();

        // Validate user’s input
        foreach (var chargeItem in request.Charge.Charges)
        {
            // Validation: ExtraOccupancy must not exceed allowed max
            if (chargeItem.ExtraOccupancy > room.MaxExtraOccupancy)
                throw new Exception(
                    $"ExtraOccupancy {chargeItem.ExtraOccupancy} exceeds max allowed {room.MaxExtraOccupancy} for this room."
                );

            // 2. Check if the same RoomId + BookingType + ExtraOccupancy already exists

            bool exists = await _context.RoomCharges.AnyAsync(rc =>
            rc.RoomId == request.Charge.RoomId &&
            rc.BookingType == request.Charge.BookingType &&
            rc.ExtraOccupancy == chargeItem.ExtraOccupancy,
            cancellationToken);

            if (exists)
                throw new Exception($"RoomCharge already exists for RoomId {request.Charge.RoomId}, BookingType {request.Charge.BookingType}, ExtraOccupancy {chargeItem.ExtraOccupancy}.");

            var roomCharge = new RoomCharge
            {
                RoomId = request.Charge.RoomId,
                BookingType = request.Charge.BookingType,
                ExtraOccupancy = chargeItem.ExtraOccupancy,
                Charges = chargeItem.Charges
            };
            _context.RoomCharges.Add(roomCharge);
            addedIds.Add(roomCharge.Id);
        }
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<List<Guid>>(addedIds, "Room Charges Added.");
    }
}
