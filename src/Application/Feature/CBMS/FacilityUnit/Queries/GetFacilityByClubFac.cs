using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Queries;

public record GetFacilityUniByClubFacQuery(Guid? ClubId, Guid? Facility) : IRequest<ApiResult<List<FacilityUnitDTO>>>;

public class GetFacilityUniByClubFacQueryHandler : IRequestHandler<GetFacilityUniByClubFacQuery, ApiResult<List<FacilityUnitDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetFacilityUniByClubFacQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<FacilityUnitDTO>>> Handle(GetFacilityUniByClubFacQuery request, CancellationToken ct)
    {
        List<FacilityUnitDTO> list = new List<FacilityUnitDTO>();
        if (request.ClubId != null)
        {
            list = await _db.Set<Domain.Entities.CBMS.FacilityUnit>()
            .Where(x => x.ClubId == request.ClubId)
            .OrderBy(x => x.Name)
            .Select(x => new FacilityUnitDTO(x.Id, x.ClubId, x.FacilityId, x.Name, x.Code, x.UnitType, "", new List<string>(), x.IsActive, x.IsDeleted))
            .ToListAsync(ct);

        }
        else if (request.Facility != null)
        {
            list = await _db.Set<Domain.Entities.CBMS.FacilityUnit>()
            .Where(x => x.FacilityId == request.Facility)
            .OrderBy(x => x.Name)
            .Select(x => new FacilityUnitDTO(x.Id, x.ClubId, x.FacilityId, x.Name, x.Code, x.UnitType,"",new List<string>(), x.IsActive, x.IsDeleted))
            .ToListAsync(ct);
        }
        else
        {
            list = await _db.Set<Domain.Entities.CBMS.FacilityUnit>()
           .Where(x => x.FacilityId == request.Facility && x.ClubId == request.Facility)
           .OrderBy(x => x.Name)
            .Select(x => new FacilityUnitDTO(x.Id, x.ClubId, x.FacilityId, x.Name, x.Code, x.UnitType, "", new List<string>(), x.IsActive, x.IsDeleted))
           .ToListAsync(ct);
        }


        if (list == null) return ApiResult<List<FacilityUnitDTO>>.Fail("Facility Unit not found.");

        return ApiResult<List<FacilityUnitDTO>>.Ok(list);
    }
}




