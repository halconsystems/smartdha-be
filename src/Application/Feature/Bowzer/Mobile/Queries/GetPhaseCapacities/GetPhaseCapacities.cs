using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Contracts.Mobile;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetPhaseCapacities;
public sealed record GetPhaseCapacitiesQuery(Guid PhaseId) : IRequest<List<CapacityDto>>;

public sealed class GetPhaseCapacitiesHandler(IOLHApplicationDbContext db)
    : IRequestHandler<GetPhaseCapacitiesQuery, List<CapacityDto>>
{
    public async Task<List<CapacityDto>> Handle(GetPhaseCapacitiesQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var items = await db.PhaseCapacities
            .Where(pc => pc.PhaseId == request.PhaseId &&
                        (pc.IsDeleted == false || pc.IsDeleted == null) &&
                        (pc.EffectiveTo == null || pc.EffectiveTo >= now) &&
                         pc.EffectiveFrom <= now)
            .Include(pc => pc.BowserCapacity)
            .ToListAsync(ct);

        return items
            .OrderBy(pc => pc.BowserCapacity.Capacity)
            .Select(pc => new CapacityDto(
                pc.BowserCapacityId,
                $"{pc.BowserCapacity.Capacity} {pc.BowserCapacity.Unit}",
                pc.BaseRate))
            .ToList();
    }
}
