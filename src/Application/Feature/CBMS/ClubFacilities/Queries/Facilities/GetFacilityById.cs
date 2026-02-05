using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.Facilities;

public record GetFacilityByIdQuery(Guid Id) : IRequest<ApiResult<FacilitiesDTO>>;

public class GetFacilityByIdQueryHandler : IRequestHandler<GetFacilityByIdQuery, ApiResult<FacilitiesDTO>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetFacilityByIdQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<FacilitiesDTO>> Handle(GetFacilityByIdQuery request, CancellationToken ct)
    {
        var facility = await _db.Set<Domain.Entities.CBMS.Facility>()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(ct);

        if (facility == null) return ApiResult<FacilitiesDTO>.Fail("Service not found.");

        var facilityMainIamges = await _db.FacilitiesImages
            .Where(x => x.FacilityId == facility.Id && x.Category == Domain.Enums.ImageCategory.Main)
            .Select(x => x.ImageURL)
            .FirstOrDefaultAsync(ct);


        var facilityIamges = await _db.FacilitiesImages
            .Where(x => x.FacilityId == facility.Id && x.Category != Domain.Enums.ImageCategory.Main)
            .Select(x => x.ImageURL)
            .ToListAsync(ct);

        var list = await _db.Set<Domain.Entities.CBMS.Facility>()
            .Where(x => x.Id == request.Id)
            .OrderBy(x => x.Name)
            .Select(x => new FacilitiesDTO(x.Id, x.Name, x.DisplayName, x.Code, x.Description, x.ClubCategoryId, facilityMainIamges, facilityIamges ,x.FoodType, x.IsActive, x.IsDeleted))
            .FirstOrDefaultAsync(ct);

        if (list == null) return ApiResult<FacilitiesDTO>.Fail("Service not found.");

        return ApiResult<FacilitiesDTO>.Ok(list);
    }
}




