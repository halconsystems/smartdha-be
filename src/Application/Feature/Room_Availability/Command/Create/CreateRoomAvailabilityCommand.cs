using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Application.Feature.Room_Availability.Command.Create;
public class CreateRoomAvailabilityCommand : IRequest<SuccessResponse<Guid>>
{
    [Required] public Guid RoomId { get; set; }

    // Separate inputs
    [Required] public DateOnly FromDate { get; set; }

    [Required] public DateOnly ToDate { get; set; }

    [Required] public AvailabilityAction Action { get; set; }
    public string? Reason { get; set; }
}


public class CreateRoomAvailabilityCommandHandler
    : IRequestHandler<CreateRoomAvailabilityCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public CreateRoomAvailabilityCommandHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<Guid>> Handle(CreateRoomAvailabilityCommand request, CancellationToken ct)
    {
        // Validate room
        var room = await _ctx.Rooms.Include(r => r.Club)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId && r.IsGloballyAvailable 
            && r.IsDeleted == false && r.IsActive == true, ct);
        if (room == null) throw new KeyNotFoundException("Room not found.");

        // Get standard times for the club
        var standardTimes = await _ctx.ClubBookingStandardTimes
            .FirstOrDefaultAsync(s => s.ClubId == room.ClubId && s.IsDeleted == false && s.IsActive == true, ct);
        if (standardTimes == null) throw new InvalidOperationException("Standard booking times not configured for this club.");

        // Combine DateOnly + TimeOnly -> DateTime in PKT (Asia/Karachi)
        var pktTz = TimeZoneInfo.FindSystemTimeZoneById(
#if WINDOWS
            "Pakistan Standard Time"   // Windows ID
#else
            "Asia/Karachi"             // Linux/Container ID
#endif
        );

        // Use DateOnly from request + TimeOnly from standard times
        var fromLocal = request.FromDate.ToDateTime(standardTimes.CheckInTime, DateTimeKind.Unspecified);
        var toLocal = request.ToDate.ToDateTime(standardTimes.CheckOutTime, DateTimeKind.Unspecified);

        // enforce that To >= From
        if (toLocal < fromLocal)
            throw new ArgumentException("To date/time must be greater than or equal to From date/time.");

        // If you want UTC storage for the full DateTimes:
        // var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromLocal, pktTz);
        // var toUtc   = TimeZoneInfo.ConvertTimeToUtc(toLocal, pktTz);

        // Check if overlaps with existing availabilities for the same room
        var hasOverlap = await _ctx.RoomAvailabilities
            .Where(a =>
                a.RoomId == request.RoomId &&
                a.IsDeleted != true &&
                a.FromDateOnly <= request.ToDate &&
                request.FromDate <= a.ToDateOnly)
            .FirstOrDefaultAsync(ct);

        if (hasOverlap != null)
        {
            throw new InvalidOperationException(
                $"This room already has an overlapping availability from " +
                $"{hasOverlap.FromDate:dd-MM-yyyy} to {hasOverlap.ToDateOnly:dd-MM-yyyy}."
            );
        }

        // Create entity (store both combined and split fields)
        var entity = new RoomAvailability
        {
            RoomId = request.RoomId,

            // Combined (keep as local PKT; or use fromUtc/toUtc if you prefer UTC)
            FromDate = fromLocal,
            ToDate = toLocal,

            // Split fields for exact matching
            FromDateOnly = request.FromDate,
            FromTimeOnly = standardTimes.CheckInTime,
            ToDateOnly = request.ToDate,
            ToTimeOnly = standardTimes.CheckOutTime,

            Action = request.Action,
            Reason = request.Reason
        };

        await _ctx.RoomAvailabilities.AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id);
    }
}


