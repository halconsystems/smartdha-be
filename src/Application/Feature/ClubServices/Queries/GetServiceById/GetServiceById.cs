using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.ClubServices.Queries.GetServiceById;
public record GetServiceByIdQuery(Guid Id) : IRequest<Services?>;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, Services?>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public GetServiceByIdQueryHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<Services?> Handle(GetServiceByIdQuery request, CancellationToken ct)
    {
        return await _ctx.Services.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == false || x.IsDeleted == null), ct);
    }
}
