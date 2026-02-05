using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.Facilities;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.Queries;

public record GetFacilityServiceUnitByIdQuery(Guid Id) : IRequest<ApiResult<FacilityServiceUnitDTO>>;

public class GetFacilityServiceUnitByIdQueryHandler : IRequestHandler<GetFacilityServiceUnitByIdQuery, ApiResult<FacilityServiceUnitDTO>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetFacilityServiceUnitByIdQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<FacilityServiceUnitDTO>> Handle(GetFacilityServiceUnitByIdQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.FacilityUnitService>()
            .Where(x => x.Id == request.Id)
            .Select(x => new FacilityServiceUnitDTO(x.Id, x.FacilityUnitId, Guid.NewGuid(), 10, x.IsEnabled,x.IsActive, x.IsDeleted))
            .FirstOrDefaultAsync(ct);

        if (list == null) return ApiResult<FacilityServiceUnitDTO>.Fail("Service not found.");

        return ApiResult<FacilityServiceUnitDTO>.Ok(list);
    }
}




