using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Queries;

public record GetFacilityUnitByClubFacQuery(
    Guid ClubId,
    Guid FacilityId
) : IRequest<ApiResult<List<FacilityUnitDTO>>>;

public class GetFacilityUnitByClubFacQueryHandler
    : IRequestHandler<GetFacilityUnitByClubFacQuery, ApiResult<List<FacilityUnitDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public GetFacilityUnitByClubFacQueryHandler(
        ICBMSApplicationDbContext db,
        IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ApiResult<List<FacilityUnitDTO>>> Handle(
         GetFacilityUnitByClubFacQuery request,
         CancellationToken ct)
    {
        var units = await _db.FacilityUnits
            .AsNoTracking()
            .Where(x =>
                x.ClubId == request.ClubId &&
                x.FacilityId == request.FacilityId &&
                x.IsDeleted != true)
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                // Unit core
                x.Id,
                x.ClubId,
                x.FacilityId,
                x.Name,
                x.Code,
                x.UnitType,
                x.IsActive,
                x.IsDeleted,

                // Club
                ClubName = x.Club.Name,

                // Facility
                FacilityName = x.Facility.Name,

                // Category
                CategoryId = x.Facility.ClubCategory.Id,
                CategoryName = x.Facility.ClubCategory.Name,

                // Services
                Services = x.FacilityUnitServices
                    .Where(s => s.IsEnabled)
                    .Select(s => s.ServiceDefinition.Name)
                    .ToList(),

                // Images (raw paths only)
                Images = x.FacilityUnitImages
                    .Select(i => new { i.ImageURL, i.Category })
                    .ToList()
            })
            .ToListAsync(ct);

        if (!units.Any())
            return ApiResult<List<FacilityUnitDTO>>
                .Fail("No facility units found.");

        var result = units.Select(x =>
        {
            var mainImagePath = x.Images
                .FirstOrDefault(i => i.Category == Domain.Enums.ImageCategory.Main)
                ?.ImageURL;

            var bannerImagePaths = x.Images
                .Where(i => i.Category != Domain.Enums.ImageCategory.Main)
                .Select(i => i.ImageURL)
                .ToList();

            var mainImageUrl = string.IsNullOrWhiteSpace(mainImagePath)
                ? null
                : _fileStorage.GetPublicUrl(mainImagePath);

            var bannerImageUrls = bannerImagePaths
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => _fileStorage.GetPublicUrl(p))
                .ToList();

            return new FacilityUnitDTO(
                x.Id,
                x.ClubId,
                x.ClubName,

                x.CategoryId,
                x.CategoryName,

                x.FacilityId,
                x.FacilityName,

                x.Name,
                x.Code,
                x.UnitType,

                mainImageUrl,
                bannerImageUrls,

                x.Services,

                x.IsActive,
                x.IsDeleted
            );
        }).ToList();

        return ApiResult<List<FacilityUnitDTO>>.Ok(result);
    }
}




