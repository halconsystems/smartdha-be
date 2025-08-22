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
        var r = c.Request ?? throw new ArgumentNullException(nameof(c.Request));

        // 1️⃣ Room must exist
        var roomExists = await _ctx.Rooms
            .AnyAsync(x => x.Id == r.RoomId && (x.IsDeleted == false || x.IsDeleted == null), ct);

        if (!roomExists)
            throw new ArgumentException("Invalid RoomId.");

        // 2️⃣ Validate all serviceIds exist
        var distinctServiceIds = r.ServiceIds?.Distinct().ToList() ?? new List<Guid>();

        var validCount = await _ctx.Services
            .CountAsync(s => distinctServiceIds.Contains(s.Id) && (s.IsDeleted == false || s.IsDeleted == null), ct);

        if (validCount != distinctServiceIds.Count)
            throw new ArgumentException("One or more ServiceIds are invalid.");

        // 3️⃣ Remove all previous mappings (hard delete)
        var existingMappings = await _ctx.ServiceMappings
            .Where(m => m.RoomId == r.RoomId)
            .ToListAsync(ct);

        _ctx.ServiceMappings.RemoveRange(existingMappings);

        // 4️⃣ Add new mappings
        foreach (var sid in distinctServiceIds)
        {
            await _ctx.ServiceMappings.AddAsync(new ServiceMapping
            {
                RoomId = r.RoomId,
                ServiceId = sid
            }, ct);
        }

        // 5️⃣ Save changes
        await _ctx.SaveChangesAsync(ct);

        return new SuccessResponse<string>("OK", "Room services updated.");
    }

}

