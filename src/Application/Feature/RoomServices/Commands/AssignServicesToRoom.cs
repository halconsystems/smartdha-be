using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.RoomServices.Commands;
public record AssignServicesToRoomCommand(AssignServicesToRoomRequest Request)
    : IRequest<SuccessResponse<string>>;

public sealed class AssignServicesToRoomCommandHandler
    : IRequestHandler<AssignServicesToRoomCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public AssignServicesToRoomCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(AssignServicesToRoomCommand c, CancellationToken ct)
    {
        var r = c.Request;
        if (r.ServiceIds == null) r.ServiceIds = new();

        // room must exist
        var roomExists = await _ctx.Rooms.AnyAsync(x => x.Id == r.RoomId && (x.IsDeleted == false || x.IsDeleted == null), ct);
        if (!roomExists) throw new ArgumentException("Invalid RoomId.");

        // validate all serviceIds exist
        var validCount = await _ctx.Services.CountAsync(s => r.ServiceIds.Contains(s.Id) && (s.IsDeleted == false || s.IsDeleted == null), ct);
        if (validCount != r.ServiceIds.Distinct().Count())
            throw new ArgumentException("One or more ServiceIds are invalid.");

        // current mappings
        var current = await _ctx.ServiceMappings
            .Where(m => m.RoomId == r.RoomId && (m.IsDeleted == false || m.IsDeleted == null))
            .ToListAsync(ct);

        var newSet = r.ServiceIds.Distinct().ToHashSet();

        // soft-remove those not in new set
        var toRemove = current.Where(m => !newSet.Contains(m.ServiceId)).ToList();
        foreach (var m in toRemove)
        {
            m.IsDeleted = true;
            m.IsActive = false;
            m.LastModified = DateTime.Now;
        }

        // add new ones
        var existingIds = current.Select(m => m.ServiceId).ToHashSet();
        var toAdd = newSet.Except(existingIds).ToList();

        foreach (var sid in toAdd)
        {
            await _ctx.ServiceMappings.AddAsync(new ServiceMapping
            {
                RoomId = r.RoomId,
                ServiceId = sid
            }, ct);
        }

        // revive previously soft-deleted same pairs (optional)
        var previouslyDeleted = await _ctx.ServiceMappings
            .Where(m => m.RoomId == r.RoomId && r.ServiceIds.Contains(m.ServiceId) && m.IsDeleted == true)
            .ToListAsync(ct);

        foreach (var m in previouslyDeleted)
        {
            if (!current.Any(x => x.ServiceId == m.ServiceId)) // only if not already active
            {
                m.IsDeleted = false;
                m.IsActive = true;
                m.LastModified = DateTime.Now;
            }
        }
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return new SuccessResponse<string>("OK", "Room services updated.");
    }
}

