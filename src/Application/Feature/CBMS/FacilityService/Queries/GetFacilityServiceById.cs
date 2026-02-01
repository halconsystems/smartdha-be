using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Queries;


public record GetFacilityServiceByIdQuery(Guid Id) : IRequest<ApiResult<FacilityServiceDTO>>;

public class GetFacilityServiceByIdQueryHandler : IRequestHandler<GetFacilityServiceByIdQuery, ApiResult<FacilityServiceDTO>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetFacilityServiceByIdQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<FacilityServiceDTO>> Handle(GetFacilityServiceByIdQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.FacilityService>()
            .OrderBy(x => x.Name)
            .Where(x => x.Id == request.Id)
            .Select(x => new FacilityServiceDTO(x.Id, x.FacilityId, x.Name, x.Code, x.IsComplimentary, x.Price, x.IsQuantityBased, x.IsActive, x.IsDeleted))
            .FirstOrDefaultAsync(ct);

        if(list == null) return ApiResult<FacilityServiceDTO>.Fail("Service not found.");


        return ApiResult<FacilityServiceDTO>.Ok(list);
    }
}





