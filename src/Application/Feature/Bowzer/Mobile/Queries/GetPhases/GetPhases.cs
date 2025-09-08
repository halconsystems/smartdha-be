using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Contracts.Mobile;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Mobile.Queries.GetPhases;
public sealed record GetPhasesQuery : IRequest<List<PhaseDto>>;

public sealed class GetPhasesHandler(IOLHApplicationDbContext db) : IRequestHandler<GetPhasesQuery, List<PhaseDto>>
{
    public async Task<List<PhaseDto>> Handle(GetPhasesQuery request, CancellationToken ct)
    {
        return await db.Phases
            .Where(p => p.IsDeleted == false || p.IsDeleted == null)
            .OrderBy(p => p.Name)
            .Select(p => new PhaseDto(p.Id, p.Name))
            .ToListAsync(ct);
    }
}
