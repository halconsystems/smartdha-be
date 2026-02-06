using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.DropDonws.Queries;
public record GetServiceDefinitionsByFacilityUnitQuery(
    Guid FacilityUnitId
) : IRequest<ApiResult<List<DropdownDto>>>;
public class GetServiceDefinitionsByFacilityUnitHandler
    : IRequestHandler<GetServiceDefinitionsByFacilityUnitQuery, ApiResult<List<DropdownDto>>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetServiceDefinitionsByFacilityUnitHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<List<DropdownDto>>> Handle(
        GetServiceDefinitionsByFacilityUnitQuery request,
        CancellationToken ct)
    {
        // 1️⃣ Get category of the unit
        var categoryId = await _db.FacilityUnits
            .Where(x => x.Id == request.FacilityUnitId && x.IsDeleted != true)
            .Select(x => x.Facility.ClubCategoryId)
            .FirstOrDefaultAsync(ct);

        if (categoryId == Guid.Empty)
            return ApiResult<List<DropdownDto>>.Fail("Invalid facility unit.");

        // 2️⃣ Get already assigned services for this unit
        var existingServiceIds = await _db.FacilityUnitServices
            .Where(x => x.FacilityUnitId == request.FacilityUnitId && x.IsEnabled)
            .Select(x => x.ServiceDefinitionId)
            .ToListAsync(ct);

        // 3️⃣ Get available services for dropdown
        var services = await _db.ServiceDefinitions
            .AsNoTracking()
            .Where(x =>
                x.ClubServiceCategoryId == categoryId &&
                x.IsDeleted != true &&
                x.IsActive == true &&
                !existingServiceIds.Contains(x.Id))
            .OrderBy(x => x.Name)
            .Select(x => new DropdownDto(
                x.Id,
                x.Name
            ))
            .ToListAsync(ct);

        return ApiResult<List<DropdownDto>>.Ok(services);
    }
}

