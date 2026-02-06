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
    private readonly IFileStorageService _file;
    public GetAllFacilitiesQueryHandler(ICBMSApplicationDbContext db,IFileStorageService fileStorage)
    {
        _db = db;
        _file = fileStorage;
    }

    public async Task<ApiResult<List<FacilitiesDTO>>> Handle(GetAllFacilitiesQuery request, CancellationToken ct)
    {
        var facility = await _db.Set<Domain.Entities.CBMS.Facility>()
            .AsNoTracking()
            .ToListAsync(ct);

        if (facility == null) return ApiResult<List<FacilitiesDTO>>.Fail("Service not found.");

        var facilityMainIamges = await _db.FacilitiesImages
            .Where(x => facility.Select(f => f.Id).Contains(x.FacilityId) && x.Category == Domain.Enums.ImageCategory.Main)
            .Select(x => new
            {
                x.FacilityId,
                x.ImageURL
            })
            .ToListAsync(ct);


        Dictionary<Guid,string> MainImagepublicUrls = new Dictionary<Guid, string>();
        if (facilityMainIamges != null)
        {
            MainImagepublicUrls = facilityMainIamges
                .Where(g => !string.IsNullOrEmpty(g.ImageURL))
                .ToDictionary(
                    img => img.FacilityId,
                    img => _file.GetPublicUrl(img.ImageURL)

                );

        }


        var facilityIamges = await _db.FacilitiesImages
            .Where(x => facility.Select(f => f.Id).Contains(x.FacilityId) && x.Category != Domain.Enums.ImageCategory.Main)
            .Select(x => new
            {
                x.FacilityId,
                x.ImageURL
            })
            .ToListAsync(ct);

        Dictionary<Guid, string> ImagespublicUrls = new Dictionary<Guid, string>();
        if (facilityIamges != null)
        {
            ImagespublicUrls = facilityIamges
               .Where(g => !string.IsNullOrEmpty(g.ImageURL))
               .ToDictionary(
                   img => img.FacilityId,
                   img => _file.GetPublicUrl(img.ImageURL)

               );
        }

        var facilities = await _db.Set<Domain.Entities.CBMS.Facility>()
            .Where(x => x.IsDeleted != true && x.IsActive == true)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        var list = facilities.Select(x => new FacilitiesDTO(
            x.Id,
            x.Name,
            x.DisplayName,
            x.Code,
            x.Description,
            x.ClubCategoryId,

            // Main Image (single)
            MainImagepublicUrls.TryGetValue(x.Id, out var mainImg)
                ? mainImg
                : null,

            // Other Images (multiple)
            ImagespublicUrls
                .Where(i => i.Key == x.Id)
                .Select(i => i.Value)
                .ToList(),

            x.FoodType,
            x.IsActive,
            x.IsDeleted
        )).ToList();


        return ApiResult<List<FacilitiesDTO>>.Ok(list);
    }
}



