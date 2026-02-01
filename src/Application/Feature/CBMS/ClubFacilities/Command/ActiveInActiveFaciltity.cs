using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Command;

public record ActiveInActiveFaciltityCommand(Guid Id, bool Active)
    : IRequest<ApiResult<bool>>;
public class ActiveInActiveFaciltityCommandHandler
    : IRequestHandler<ActiveInActiveFaciltityCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public ActiveInActiveFaciltityCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        ActiveInActiveFaciltityCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.Facility>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Facility not found.");

        entity.IsActive = request.Active;


        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Facility Updated successfully.");
    }
}
