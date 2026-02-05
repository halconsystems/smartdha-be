using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.Facilities;

public record FacilitiesDTO(Guid Id, string Name,string DisplayName ,string Code, string? Description, Guid ClubCategoryId, string? ImageURL,List<string>? BannerURL, FoodType? FoodType, bool? IsActive, bool? IsDeleted);
public record GetAllFacilitiesQuery() : IRequest<ApiResult<List<FacilitiesDTO>>>;

public class GetAllFacilitiesQueryHandler : IRequestHandler<GetAllFacilitiesQuery, ApiResult<List<FacilitiesDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetAllFacilitiesQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<FacilitiesDTO>>> Handle(GetAllFacilitiesQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.Facility>()
            .Where(x=> x.IsDeleted != true && x.IsActive==true)
            .OrderBy(x => x.Name)
            .Select(x => new FacilitiesDTO(x.Id, x.Name, x.DisplayName, x.Code,x.Description ,x.ClubCategoryId,x.ImageURL,new List<string>(),x.FoodType, x.IsActive, x.IsDeleted))
            .ToListAsync(ct);

        return ApiResult<List<FacilitiesDTO>>.Ok(list);
    }
}



