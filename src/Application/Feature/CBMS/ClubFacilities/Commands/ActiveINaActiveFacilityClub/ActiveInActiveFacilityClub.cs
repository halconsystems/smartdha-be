using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.ActiveINaActiveFacilityClub;

public record ActiveInActiveFacilityClubCommand(Guid Id, bool Active)
    : IRequest<ApiResult<bool>>;
public class ActiveInActiveFacilityClubCommandHandler
    : IRequestHandler<ActiveInActiveFacilityClubCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public ActiveInActiveFacilityClubCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        ActiveInActiveFacilityClubCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.ClubFacility>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Club Facility not found.");

        entity.IsActive = request.Active;


        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Club Facility Updated successfully.");
    }
}
