using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.Facilities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Queries;


public record GetFacilityUnitByIdQuery(Guid Id) : IRequest<ApiResult<FacilityUnitDTO>>;

public class GetFacilityUnitByIdQueryHandler : IRequestHandler<GetFacilityUnitByIdQuery, ApiResult<FacilityUnitDTO>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetFacilityUnitByIdQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<FacilityUnitDTO>> Handle(GetFacilityUnitByIdQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.FacilityUnit>()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(ct);

        if (list == null) return ApiResult<FacilityUnitDTO>.Fail("Facility Unit not found.");

        var facilityMainIamges = await _db.FacilityUnitImages
           .Where(x => x.FacilityUnitId == list.Id && x.Category == Domain.Enums.ImageCategory.Main)
           .Select(x => x.ImageURL)
           .FirstOrDefaultAsync(ct);


        var facilityIamges = await _db.FacilityUnitImages
            .Where(x => x.FacilityUnitId == list.Id && x.Category != Domain.Enums.ImageCategory.Main)
            .Select(x => x.ImageURL)
            .ToListAsync(ct);

        var result = await _db.Set<Domain.Entities.CBMS.FacilityUnit>()
            .Where(x => x.Id == request.Id)
            .OrderBy(x => x.Name)
            .Select(x => new FacilityUnitDTO(x.Id, x.ClubId, x.FacilityId, x.Name, x.Code, x.UnitType,facilityMainIamges, facilityIamges,x.IsActive, x.IsDeleted))
            .FirstOrDefaultAsync(ct);

        if (result == null) return ApiResult<FacilityUnitDTO>.Fail("Facility Unit not found.");

        return ApiResult<FacilityUnitDTO>.Ok(result);
    }
}




