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
    [Required] public DateOnly SlotDate { get; set; }
    [Required] public TimeOnly FromTime { get; set; }
    [Required] public TimeOnly ToTime { get; set; }

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

        // enforce that To >= From
        if (request.FromTime > request.ToTime)
            throw new ArgumentException("To date/time must be greater than or equal to From date/time.");

        // If you want UTC storage for the full DateTimes:
        // var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromLocal, pktTz);
        // var toUtc   = TimeZoneInfo.ConvertTimeToUtc(toLocal, pktTz);

        // Check if overlaps with existing availabilities for the same room
        var hasOverlap = await _ctx.GroundSlots
     .AnyAsync(a =>
         a.GroundId == request.GroundId &&
         a.IsDeleted != true &&
         a.FromTimeOnly < request.ToTime &&
         a.ToTimeOnly > request.FromTime,
         ct);


        if (hasOverlap)
        {
            throw new InvalidOperationException(
                $"This room already has an overlapping availability from "
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
            SlotDate = request.SlotDate,


            FromTimeOnly = request.FromTime,
            ToTimeOnly = request.ToTime,

            Action = request.Action,
            Reason = request.Reason
        };

        await _ctx.GroundSlots.AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id);
    }
}

