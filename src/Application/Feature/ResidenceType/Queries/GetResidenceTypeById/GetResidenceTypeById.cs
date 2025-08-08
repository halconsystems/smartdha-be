using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceType.Queries.GetResidenceTypeById;
public record GetResidenceTypeByIdQuery(Guid Id) : IRequest<DHAFacilitationAPIs.Domain.Entities.ResidenceType?>;

public class GetResidenceTypeByIdQueryHandler : IRequestHandler<GetResidenceTypeByIdQuery, DHAFacilitationAPIs.Domain.Entities.ResidenceType?>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public GetResidenceTypeByIdQueryHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<DHAFacilitationAPIs.Domain.Entities.ResidenceType?> Handle(GetResidenceTypeByIdQuery request, CancellationToken ct)
    {
        return await _ctx.ResidenceTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == false || x.IsDeleted == null), ct);
    }
}
