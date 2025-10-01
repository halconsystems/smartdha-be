using System.Threading;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.DeleteRoomCharges;

public class DeleteRoomCharges : IRequest<SuccessResponse<string>>
{
    public Guid RoomId { get; set; }
    public RoomBookingType BookingType { get; set; } = RoomBookingType.Self;
    public int ExtraOccupancy { get; set; }
    public bool HardDelete { get; set; } = false;
}

public class DeleteRoomChargesHandler : IRequestHandler<DeleteRoomCharges, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public DeleteRoomChargesHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteRoomCharges request, CancellationToken ct)
    {
        // Find existing RoomCharge
        var entity = await _ctx.RoomCharges.FirstOrDefaultAsync(rc =>
            rc.RoomId == request.RoomId &&
            rc.BookingType == request.BookingType &&
            rc.ExtraOccupancy == request.ExtraOccupancy &&
            rc.IsDeleted == false && rc.IsActive == true,
            ct);

        if (entity is null)
            throw new KeyNotFoundException("RoomCharge not found.");

        if (request.HardDelete)
        {
            _ctx.RoomCharges.Remove(entity);
        }
        else
        {
            entity.IsDeleted = true;      // assumes soft delete flags exist in RoomCharge entity
            entity.IsActive = false;
            entity.LastModified = DateTime.Now;
        }

        await _ctx.SaveChangesAsync(ct);

        return Success.Delete(entity.Id.ToString());
    }
}
