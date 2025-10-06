using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Room_Availability.Command.Update;
public class UpdateRoomAvailabilityCommand : IRequest<SuccessResponse<Guid>>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public DateOnly FromDate { get; set; }

    [Required]
    public DateOnly ToDate { get; set; }

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
        var room = await _ctx.Rooms.Include(r => r.Club)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId && r.IsGloballyAvailable
            && r.IsDeleted == false && r.IsActive == true, ct);
        if (room == null) throw new KeyNotFoundException("Room not found.");

        // Get standard times for the club
        var standardTimes = await _ctx.ClubBookingStandardTimes
            .FirstOrDefaultAsync(s => s.ClubId == room.ClubId && s.IsDeleted == false && s.IsActive == true, ct);
        if (standardTimes == null) throw new InvalidOperationException("Standard booking times not configured for this club.");

        // Use DateOnly from request + TimeOnly from standard times
        var fromLocal = request.FromDate.ToDateTime(standardTimes.CheckInTime, DateTimeKind.Unspecified);
        var toLocal = request.ToDate.ToDateTime(standardTimes.CheckOutTime, DateTimeKind.Unspecified);

        // enforce that To >= From
        if (toLocal < fromLocal)
            throw new ArgumentException("To date/time must be greater than or equal to From date/time.");

        // Check if overlaps with existing availabilities for the same room
        var hasOverlap = await _ctx.RoomAvailabilities
            .Where(a =>
                a.RoomId == request.RoomId &&
                a.IsDeleted != true &&
                a.FromDate < toLocal &&
                fromLocal < a.ToDate)
            .FirstOrDefaultAsync(ct);

        if (hasOverlap != null)
        {
            throw new InvalidOperationException(
                $"This room already has an overlapping availability from " +
                $"{hasOverlap.FromDate:dd-MM-yyyy HH:mm:ss} to {hasOverlap.ToDate:dd-MM-yyyy HH:mm:ss}."
            );
        }

        // Update Availability
        entity.RoomId = request.RoomId;
        entity.FromDateOnly = request.FromDate;
        entity.FromTimeOnly = standardTimes.CheckInTime;
        entity.FromDate = fromLocal;
        entity.ToDateOnly = request.ToDate;
        entity.ToTimeOnly = standardTimes.CheckOutTime;
        entity.ToDate = toLocal;
        entity.Action = request.Action;
        entity.Reason = request.Reason;
        entity.LastModified = DateTime.UtcNow;

        await _ctx.SaveChangesAsync(ct);

        return Success.Update(entity.Id, "Room availability updated successfully.");
    }
}

