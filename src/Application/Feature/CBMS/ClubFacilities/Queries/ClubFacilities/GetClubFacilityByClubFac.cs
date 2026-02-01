using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Queries.ClubFacilities;

public record GetClubFacilityByClubFacQuery(Guid? ClubId, Guid? Facility) : IRequest<ApiResult<List<ClubFacilitiesDTO>>>;

public class GetClubFacilityByClubFacQueryHandler : IRequestHandler<GetClubFacilityByClubFacQuery, ApiResult<List<ClubFacilitiesDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetClubFacilityByClubFacQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<ClubFacilitiesDTO>>> Handle(GetClubFacilityByClubFacQuery request, CancellationToken ct)
    {
        List<ClubFacilitiesDTO> list = new List<ClubFacilitiesDTO>();
        if (request.ClubId != null)
        {
            list = await _db.Set<Domain.Entities.CBMS.ClubFacility>()
            .Where(x => x.ClubId == request.ClubId)
            .OrderBy(x => x.Price)
            .Select(x => new ClubFacilitiesDTO(x.Id, x.FacilityId, x.ClubId, x.Price, x.IsAvailable, x.IsPriceVisible, x.HasAction, x.ActionName, x.ActionType, x.IsActive, x.IsDeleted))
            .ToListAsync(ct);

        }
        else if(request.Facility != null)
        {
            list = await _db.Set<Domain.Entities.CBMS.ClubFacility>()
            .Where(x => x.FacilityId == request.Facility)
            .OrderBy(x => x.Price)
            .Select(x => new ClubFacilitiesDTO(x.Id, x.FacilityId, x.ClubId, x.Price, x.IsAvailable, x.IsPriceVisible, x.HasAction, x.ActionName, x.ActionType, x.IsActive, x.IsDeleted))
            .ToListAsync(ct);
        }
        else
        {
            list = await _db.Set<Domain.Entities.CBMS.ClubFacility>()
           .Where(x => x.FacilityId == request.Facility && x.ClubId == request.Facility)
           .OrderBy(x => x.Price)
           .Select(x => new ClubFacilitiesDTO(x.Id, x.FacilityId, x.ClubId, x.Price, x.IsAvailable, x.IsPriceVisible, x.HasAction, x.ActionName, x.ActionType, x.IsActive, x.IsDeleted))
           .ToListAsync(ct);
        }


        if (list == null) return ApiResult<List<ClubFacilitiesDTO>>.Fail("Club Facility not found.");

        return ApiResult<List<ClubFacilitiesDTO>>.Ok(list);
    }
}




