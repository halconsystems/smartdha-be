using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;

public record GetClubFacilityByClubFacQuery(
    Guid ClubId,
    Guid FacilityId
) : IRequest<ApiResult<List<ClubFacilitiesDTO>>>;

public class GetClubFacilityByClubFacQueryHandler
    : IRequestHandler<GetClubFacilityByClubFacQuery, ApiResult<List<ClubFacilitiesDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetClubFacilityByClubFacQueryHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<List<ClubFacilitiesDTO>>> Handle(
        GetClubFacilityByClubFacQuery request,
        CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.ClubFacility>()
            .AsNoTracking()
            .Where(x =>
                x.ClubId == request.ClubId &&        // ✅ AND condition
                x.FacilityId == request.FacilityId &&
                x.IsDeleted != true
            )
            .OrderBy(x => x.Price)
            .Select(x => new ClubFacilitiesDTO(
                x.Id,
                x.ClubId,
                x.FacilityId,
                x.Facility.Name,
                x.Facility.ClubCategory.Name,
                x.Price,
                x.IsAvailable,
                x.IsPriceVisible,
                x.HasAction,
                x.ActionName,
                x.ActionType,
                x.IsActive,
                x.IsDeleted
            ))
            .ToListAsync(ct);

        if (!list.Any())
            return ApiResult<List<ClubFacilitiesDTO>>.Fail("Club facility not found.");

        return ApiResult<List<ClubFacilitiesDTO>>.Ok(list);
    }
}





