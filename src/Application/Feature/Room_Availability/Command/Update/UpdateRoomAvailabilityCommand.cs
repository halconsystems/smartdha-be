using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Room_Availability.Command.Update;
public class UpdateRoomAvailabilityCommand : IRequest<SuccessResponse<Guid>>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    [Required]
    public AvailabilityAction Action { get; set; }

    public string? Reason { get; set; }
}
public class UpdateRoomAvailabilityCommandHandler : IRequestHandler<UpdateRoomAvailabilityCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public UpdateRoomAvailabilityCommandHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateRoomAvailabilityCommand request, CancellationToken ct)
    {
        var entity = await _ctx.RoomAvailabilities.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null)
            throw new KeyNotFoundException("Room availability record not found.");

        // Optional: Validate RoomId
        var roomExists = await _ctx.Rooms.AnyAsync(r => r.Id == request.RoomId, ct);
        if (!roomExists)
            throw new KeyNotFoundException("Room not found.");

        // Update Availability
        entity.RoomId = request.RoomId;
        entity.FromDate = request.FromDate;
        entity.ToDate = request.ToDate;
        entity.Action = request.Action;
        entity.Reason = request.Reason;
        entity.LastModified = DateTime.UtcNow;

        await _ctx.SaveChangesAsync(ct);

        return Success.Update(entity.Id, "Room availability updated successfully.");
    }
}

