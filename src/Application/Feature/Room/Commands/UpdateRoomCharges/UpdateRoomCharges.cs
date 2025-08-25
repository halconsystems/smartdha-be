using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomCharges.Dtos;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.UpdateRoomCharges;

public record UpdateRoomCharges(RoomChargesDto Charge) : IRequest<SuccessResponse<List<Guid>>>;

public class UpdateRoomChargesHandler : IRequestHandler<UpdateRoomCharges, SuccessResponse<List<Guid>>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public UpdateRoomChargesHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<Guid>>> Handle(UpdateRoomCharges request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
            .Where(r => r.Id == request.Charge.RoomId)
            .FirstOrDefaultAsync(cancellationToken);

        if (room == null)
            throw new Exception($"Room with ID {request.Charge.RoomId} not found.");

        var updatedIds = new List<Guid>();

        foreach (var chargeItem in request.Charge.Charges)
        {
            // Validate: ExtraOccupancy must not exceed allowed max
            if (chargeItem.ExtraOccupancy > room.MaxExtraOccupancy)
                throw new Exception(
                    $"ExtraOccupancy {chargeItem.ExtraOccupancy} exceeds max allowed {room.MaxExtraOccupancy} for this room."
                );

            // Find existing RoomCharge
            var existingCharge = await _context.RoomCharges.FirstOrDefaultAsync(rc =>
                rc.RoomId == request.Charge.RoomId &&
                rc.BookingType == request.Charge.BookingType &&
                rc.ExtraOccupancy == chargeItem.ExtraOccupancy,
                cancellationToken);

            if (existingCharge == null)
            {
                throw new Exception($"RoomCharge does not exist for RoomId {request.Charge.RoomId}, BookingType {request.Charge.BookingType}, ExtraOccupancy {chargeItem.ExtraOccupancy}.");
            }

            // Update values
            existingCharge.Charges = chargeItem.Charges;

            updatedIds.Add(existingCharge.Id);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<List<Guid>>(updatedIds, "Room Charges Updated.");
    }
}
