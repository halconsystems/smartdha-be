using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundSlots.Command;


public class UpdateGroundSlotsCommand : IRequest<SuccessResponse<Guid>>
{
    public Guid Id { get; set; }
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

public class UpdateGroundSlotsCommandHandler
    : IRequestHandler<UpdateGroundSlotsCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public UpdateGroundSlotsCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<Guid>> Handle(UpdateGroundSlotsCommand request, CancellationToken ct)
    {
        var GroundSlot = await _ctx.GroundSlots.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (GroundSlot == null) throw new KeyNotFoundException("Slot not found.");
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
                fromLocal < a.ToDate &&
                a.Id == request.Id)
            .FirstOrDefaultAsync(ct);

        if (hasOverlap != null)
        {
            throw new InvalidOperationException(
                $"This room already has an overlapping availability from " +
                $"{hasOverlap.FromDate:dd-MM-yyyy HH:mm:ss} to {hasOverlap.ToDate:dd-MM-yyyy HH:mm:ss}."
            );
        }

        GroundSlot.SlotName = request.Slotname;
        GroundSlot.DisplayName = request.DisplayName;
        GroundSlot.SlotPrice = request.SlotPrice;
        GroundSlot.GroundId = request.GroundId;
        GroundSlot.FromDate = fromLocal;
        GroundSlot.ToDate = toLocal;
        GroundSlot.FromDateOnly = request.FromDate;
        GroundSlot.FromTimeOnly = standardTimes.CheckInTime;
        GroundSlot.ToDateOnly = request.ToDate;
        GroundSlot.ToTimeOnly = standardTimes.CheckOutTime;
        GroundSlot.Action = request.Action;
        GroundSlot.Reason = request.Reason;

        await _ctx.SaveChangesAsync(ct);

        return Success.Created(GroundSlot.Id);
    }
}

