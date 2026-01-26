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
    [Required] public DateOnly SlotDate { get; set; }
    [Required] public TimeOnly FromTime { get; set; }
    [Required] public TimeOnly ToTime { get; set; }
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



        // Check if overlaps with existing availabilities for the same room
        var hasOverlap = await _ctx.GroundSlots
     .AnyAsync(a =>
         a.GroundId == request.GroundId &&
         a.IsDeleted != true &&
         a.SlotDate == request.SlotDate &&
         a.FromTimeOnly < request.ToTime &&
         a.ToTimeOnly > request.FromTime,
         ct);


        if (hasOverlap)
        {
            throw new InvalidOperationException(
                $"This room already has an overlapping availability from "
            );
        }

        GroundSlot.SlotName = request.Slotname;
        GroundSlot.DisplayName = request.DisplayName;
        GroundSlot.SlotPrice = request.SlotPrice;
        GroundSlot.GroundId = request.GroundId;
        GroundSlot.SlotDate = request.SlotDate;
        GroundSlot.FromTimeOnly = request.FromTime;
        GroundSlot.ToTimeOnly = request.ToTime;
        GroundSlot.Action = request.Action;
        GroundSlot.Reason = request.Reason;

        await _ctx.SaveChangesAsync(ct);

        return Success.Created(GroundSlot.Id);
    }
}

