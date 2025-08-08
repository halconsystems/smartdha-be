using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.ClubServices.Queries.GetServices;
public record GetServicesQuery(bool IncludeInactive = false, int Page = 1, int PageSize = 50)
    : IRequest<SuccessResponse<List<ServiceDto>>>;

public class GetServicesQueryHandler
    : IRequestHandler<GetServicesQuery, SuccessResponse<List<ServiceDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public GetServicesQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper)
        => (_ctx, _mapper) = (ctx, mapper);

    public async Task<SuccessResponse<List<ServiceDto>>> Handle(GetServicesQuery request, CancellationToken ct)
    {
        var q = _ctx.Services
            .AsNoTracking()
            .Where(x => x.IsDeleted == false || x.IsDeleted == null);

        if (!request.IncludeInactive)
            q = q.Where(x => x.IsActive == true || x.IsActive == null);

        var items = await q
            .OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Success.Ok(items);
    }
}

