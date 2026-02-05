using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.Facilities;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;

public record GetClubFacilitiesByClubIdQuery(Guid ClubId)
    : IRequest<ApiResult<List<ClubFacilitiesDTO>>>;


public class GetClubFacilitiesByClubIdQueryHandler
    : IRequestHandler<GetClubFacilitiesByClubIdQuery, ApiResult<List<ClubFacilitiesDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetClubFacilitiesByClubIdQueryHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<List<ClubFacilitiesDTO>>> Handle(
        GetClubFacilitiesByClubIdQuery request,
        CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.ClubFacility>()
            .AsNoTracking()
            .Where(x =>
                x.ClubId == request.ClubId &&
                x.IsDeleted != true &&
                x.IsActive  ==true)
            .OrderBy(x => x.Price)
            .Select(x => new ClubFacilitiesDTO(
                x.Id,
                x.ClubId,
                x.FacilityId,
                x.Facility.Name,                 // FacilityName
                x.Facility.ClubCategory.Name,    // CategoryName
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
            return ApiResult<List<ClubFacilitiesDTO>>.Fail("No facilities found for this club.");

        return ApiResult<List<ClubFacilitiesDTO>>.Ok(list);
    }
}



