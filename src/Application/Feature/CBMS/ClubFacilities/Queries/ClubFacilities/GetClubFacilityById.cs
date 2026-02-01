using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.Facilities;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;

public record GetClubFacilityByIdQuery(Guid Id) : IRequest<ApiResult<ClubFacilitiesDTO>>;

public class GetClubFacilityByIdQueryHandler : IRequestHandler<GetClubFacilityByIdQuery, ApiResult<ClubFacilitiesDTO>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetClubFacilityByIdQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<ClubFacilitiesDTO>> Handle(GetClubFacilityByIdQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.ClubFacility>()
            .Where(x => x.Id == request.Id)
            .OrderBy(x => x.Price)
            .Select(x => new ClubFacilitiesDTO(x.Id, x.FacilityId, x.ClubId, x.Price, x.IsAvailable, x.IsPriceVisible, x.HasAction, x.ActionName, x.ActionType, x.IsActive, x.IsDeleted))
            .FirstOrDefaultAsync(ct);

        if (list == null) return ApiResult<ClubFacilitiesDTO>.Fail("Club Facility not found.");

        return ApiResult<ClubFacilitiesDTO>.Ok(list);
    }
}



