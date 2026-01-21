using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundImages.Command;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundSlots.Command;

public class AddGroundSlotCommand : IRequest<SuccessResponse<Guid>>
{

    public Guid GroundId { get; set; }
    [Required]
    public string Slotname { get; set; } = default!;
    [Required]
    public string DisplayName { get; set; } = default!;
    [Required]
    public string SlotPrice { get; set; } = default!;
    [Required] public DateOnly FromDate { get; set; }
    [Required] public DateOnly ToDate { get; set; }

    [Required] public AvailabilityAction Action { get; set; }
    public string? Reason { get; set; }
}

public class AddGroundSlotCommandHandler
    : IRequestHandler<AddGroundSlotCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public AddGroundSlotCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<Guid>> Handle(AddGroundSlotCommand request, CancellationToken ct)
    {
        // Validate room
        var ground = await _ctx.Grounds
            .FirstOrDefaultAsync(r => r.Id == request.GroundId
            && r.IsDeleted == false && r.IsActive == true, ct);
        if (ground == null) throw new KeyNotFoundException("Ground not found.");

        // Get standard times for the club
        var standardTimes = await _ctx.GroundStandtardTimes
            .FirstOrDefaultAsync(s => s.GroundId == ground.Id && s.IsDeleted == false && s.IsActive == true, ct);
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
        var hasOverlap = await _ctx.GroundSlots
            .Where(a =>
                a.GroundId == request.GroundId &&
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

        // Create entity (store both combined and split fields)
        var entity = new Domain.Entities.GBMS.GroundSlots
        {
            SlotName = request.Slotname,
            DisplayName = request.DisplayName,
            SlotPrice = request.SlotPrice,
            GroundId = request.GroundId,
            Code = request.DisplayName.Substring(0, request.DisplayName.Length / 2).ToUpper(),
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

        await _ctx.GroundSlots.AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id);
    }
}

