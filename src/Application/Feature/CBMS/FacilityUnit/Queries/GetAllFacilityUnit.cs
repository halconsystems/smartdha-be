using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Queries;

public record FacilityUnitDTO(Guid Id, Guid ClubId, Guid FacillityId, string? Name, string? Code,string? UnitType,string? MainImage, List<string>? BannerImage, bool? IsActive, bool? IsDeleted);
public record GetAllFaciltiUnitQuery() : IRequest<ApiResult<List<FacilityUnitDTO>>>;

public class GetAllFaciltiUnitQueryHandler : IRequestHandler<GetAllFaciltiUnitQuery, ApiResult<List<FacilityUnitDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetAllFaciltiUnitQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<FacilityUnitDTO>>> Handle(GetAllFaciltiUnitQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.FacilityUnit>()
            .OrderBy(x => x.Name)
            .Select(x => new FacilityUnitDTO(x.Id, x.ClubId, x.FacilityId , x.Name, x.Code, x.UnitType,"",new List<string>(), x.IsActive, x.IsDeleted))
            .ToListAsync(ct);

        return ApiResult<List<FacilityUnitDTO>>.Ok(list);
    }
}



