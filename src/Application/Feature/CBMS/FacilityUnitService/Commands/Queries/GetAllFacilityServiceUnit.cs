using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.Queries;


public record FacilityServiceUnitDTO(Guid Id, Guid FacilityUnitId,Guid FacilityServiceId, decimal? OverridePrice, bool IsEnabled, bool? IsActive, bool? IsDeleted);
public record GetAllFacilityServiceUnitQuery() : IRequest<ApiResult<List<FacilityServiceUnitDTO>>>;

public class GetAllFacilityUnitBookingConfigQueryHandler : IRequestHandler<GetAllFacilityServiceUnitQuery, ApiResult<List<FacilityServiceUnitDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetAllFacilityUnitBookingConfigQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<FacilityServiceUnitDTO>>> Handle(GetAllFacilityServiceUnitQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.FacilityUnitService>()
            .OrderBy(x => x.OverridePrice)
            .Select(x => new FacilityServiceUnitDTO(x.Id, x.FacilityUnitId, x.FacilityServiceId, x.OverridePrice, x.IsEnabled, x.IsActive, x.IsDeleted))
            .ToListAsync(ct);

        return ApiResult<List<FacilityServiceUnitDTO>>.Ok(list);
    }
}




