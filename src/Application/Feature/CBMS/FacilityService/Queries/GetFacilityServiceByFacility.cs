using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Queries;

public record GetFacilityServiceByFacilityQuery(Guid FacilityId) : IRequest<ApiResult<List<FacilityServiceDTO>>>;

public class GetFacilityServiceByFacilityQueryQueryHandler : IRequestHandler<GetFacilityServiceByFacilityQuery, ApiResult<List<FacilityServiceDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetFacilityServiceByFacilityQueryQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<FacilityServiceDTO>>> Handle(GetFacilityServiceByFacilityQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.FacilityService>()
            .OrderBy(x => x.Name)
            .Where(x => x.FacilityId == request.FacilityId)
            .Select(x => new FacilityServiceDTO(x.Id, x.FacilityId, x.Name, x.Code, x.IsComplimentary, x.Price, x.IsQuantityBased, x.IsActive, x.IsDeleted))
            .ToListAsync(ct);

        if (list == null) return ApiResult<List<FacilityServiceDTO>>.Fail("Service not found.");


        return ApiResult<List<FacilityServiceDTO>>.Ok(list);
    }
}






