using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetEmergencyTypes;

public record GetEmergencyTypesQuery : IRequest<List<EmergencyTypeDto>>;

public class GetEmergencyTypesQueryHandler : IRequestHandler<GetEmergencyTypesQuery, List<EmergencyTypeDto>>
{
    private readonly IApplicationDbContext _ctx;
    public GetEmergencyTypesQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<List<EmergencyTypeDto>> Handle(GetEmergencyTypesQuery request, CancellationToken ct)
     => await _ctx.EmergencyTypes.AsNoTracking()
         .IgnoreQueryFilters()                    // bypasses the global IsDeleted filter
         .Where(x => x.IsDeleted != true)         // safe local filter
         .OrderBy(x => x.Code)
         .Select(x => new EmergencyTypeDto(x.Id, x.Code, x.Name, x.HelplineNumber, x.Description))
         .ToListAsync(ct);

}

