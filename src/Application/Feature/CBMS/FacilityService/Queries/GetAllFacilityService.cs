using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubCategories.Queries;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Queries;

public record FacilityServiceDTO(Guid Id, Guid FacilityId,string Name, string Code, bool IsComplimentary, decimal Price, bool IsQuantityBased, bool? IsActive, bool? IsDeleted);
public record GetAllFacilityServiceQuery() : IRequest<ApiResult<List<FacilityServiceDTO>>>;

public class GetAllFacilityServiceQueryHandler : IRequestHandler<GetAllFacilityServiceQuery, ApiResult<List<FacilityServiceDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetAllFacilityServiceQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<FacilityServiceDTO>>> Handle(GetAllFacilityServiceQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.FacilityService>()
            .OrderBy(x => x.Name)
            .Select(x => new FacilityServiceDTO(x.Id,x.FacilityId, x.Name, x.Code,x.IsComplimentary,x.Price,x.IsQuantityBased, x.IsActive, x.IsDeleted))
            .ToListAsync(ct);

        return ApiResult<List<FacilityServiceDTO>>.Ok(list);
    }
}




