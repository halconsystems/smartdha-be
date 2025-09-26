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
    [Required] public TimeOnly FromTime { get; set; }

    [Required] public DateOnly ToDate { get; set; }
    [Required] public TimeOnly ToTime { get; set; }

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
        // 1) Validate room
        var roomExists = await _ctx.Rooms.AnyAsync(r => r.Id == request.RoomId, ct);
        if (!roomExists) throw new KeyNotFoundException("Room not found.");

        // 2) Combine DateOnly + TimeOnly -> DateTime in PKT (Asia/Karachi)
        var pktTz = TimeZoneInfo.FindSystemTimeZoneById(
#if WINDOWS
            "Pakistan Standard Time"   // Windows ID
#else
            "Asia/Karachi"             // Linux/Container ID
#endif
        );

        DateTime fromLocal = request.FromDate.ToDateTime(request.FromTime, DateTimeKind.Unspecified);
        DateTime toLocal = request.ToDate.ToDateTime(request.ToTime, DateTimeKind.Unspecified);

        // Optional: enforce that To >= From
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
                a.FromDate <= toLocal &&
                fromLocal <= a.ToDate)
            .FirstOrDefaultAsync(ct);

        if (hasOverlap != null)
        {
            throw new InvalidOperationException(
                $"This room already has an overlapping availability from " +
                $"{hasOverlap.FromDate:dd-MM-yyyy HH:mm} to {hasOverlap.ToDate:dd-MM-yyyy HH:mm}."
            );
        }

       

        // 3) Create entity (store both combined and split fields)
        var entity = new RoomAvailability
        {
            RoomId = request.RoomId,

            // Combined (keep as local PKT; or use fromUtc/toUtc if you prefer UTC)
            FromDate = fromLocal,
            ToDate = toLocal,

            // Split fields for exact matching
            FromDateOnly = request.FromDate,
            FromTimeOnly = request.FromTime,
            ToDateOnly = request.ToDate,
            ToTimeOnly = request.ToTime,

            Action = request.Action,
            Reason = request.Reason
        };

        await _ctx.RoomAvailabilities.AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id);
    }
}


