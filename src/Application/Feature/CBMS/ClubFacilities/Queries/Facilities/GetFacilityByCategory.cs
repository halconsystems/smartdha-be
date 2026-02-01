using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.Facilities;

public record GetFacilityByCategoryQuery(Guid CategoryId) : IRequest<ApiResult<List<FacilitiesDTO>>>;

public class GetFacilityByCategoryQueryHandler : IRequestHandler<GetFacilityByCategoryQuery, ApiResult<List<FacilitiesDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetFacilityByCategoryQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<FacilitiesDTO>>> Handle(GetFacilityByCategoryQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.Facility>()
            .Where(x => x.ClubCategoryId == request.CategoryId)
            .OrderBy(x => x.Name)
            .Select(x => new FacilitiesDTO(x.Id, x.Name, x.DisplayName, x.Code, x.Description, x.ClubCategoryId, x.ImageURL, new List<string>(), x.FoodType, x.IsActive, x.IsDeleted))
            .ToListAsync(ct);

        return ApiResult<List<FacilitiesDTO>>.Ok(list);
    }
}




