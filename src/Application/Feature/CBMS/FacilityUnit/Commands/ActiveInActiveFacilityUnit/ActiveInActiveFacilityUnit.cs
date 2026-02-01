using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Commands.ActiveInActiveFacilityUnit;
public record ActiveInActiveFacilityUnitCommand(Guid Id, bool Active)
    : IRequest<ApiResult<bool>>;
public class ActiveInActiveFacilityUnitCommandHandler
    : IRequestHandler<ActiveInActiveFacilityUnitCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public ActiveInActiveFacilityUnitCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        ActiveInActiveFacilityUnitCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.FacilityUnit>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Facility Unit not found.");

        entity.IsActive = request.Active;


        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Facility Unit Updated successfully.");
    }
}
