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

public class GetFacilityUnitByIdQueryHandler
    : IRequestHandler<GetFacilityUnitByIdQuery, ApiResult<FacilityUnitDTO>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public GetFacilityUnitByIdQueryHandler(ICBMSApplicationDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ApiResult<FacilityUnitDTO>> Handle(
     GetFacilityUnitByIdQuery request,
     CancellationToken ct)
    {
        var unit = await _db.FacilityUnits
            .AsNoTracking()
            .Where(x => x.Id == request.Id && x.IsDeleted != true)
            .Select(x => new
            {
                Unit = x,
                Services = x.FacilityUnitServices
                    .Where(s => s.IsEnabled)
                    .Select(s => s.ServiceDefinition.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (unit == null)
            return ApiResult<FacilityUnitDTO>.Fail("Facility Unit not found.");

        // 🔹 Load images (raw paths)
        var images = await _db.FacilityUnitImages
            .Where(x => x.FacilityUnitId == request.Id)
            .Select(x => new { x.ImageURL, x.Category })
            .ToListAsync(ct);

        var mainImagePath = images
            .FirstOrDefault(x => x.Category == Domain.Enums.ImageCategory.Main)
            ?.ImageURL;

        var bannerImagePaths = images
            .Where(x => x.Category != Domain.Enums.ImageCategory.Main)
            .Select(x => x.ImageURL)
            .ToList();

        // ✅ Convert to PUBLIC URLs
        var mainImageUrl = string.IsNullOrWhiteSpace(mainImagePath) ? null  : _fileStorage.GetPublicUrl(mainImagePath);

        var bannerImageUrls = bannerImagePaths
            .Select(p => _fileStorage.GetPublicUrl(p))
            .ToList();

        var dto = new FacilityUnitDTO(
            unit.Unit.Id,
            unit.Unit.ClubId,
            unit.Unit.Club.Name,

            unit.Unit.Facility.ClubCategory.Id,
            unit.Unit.Facility.ClubCategory.Name,

            unit.Unit.FacilityId,
            unit.Unit.Facility.Name,

            unit.Unit.Name,
            unit.Unit.Code,
            unit.Unit.UnitType,

            mainImageUrl,          // ✅ now included
            bannerImageUrls,       // ✅ now included

            unit.Services,

            unit.Unit.IsActive,
            unit.Unit.IsDeleted
        );

        return ApiResult<FacilityUnitDTO>.Ok(dto);
    }

}





