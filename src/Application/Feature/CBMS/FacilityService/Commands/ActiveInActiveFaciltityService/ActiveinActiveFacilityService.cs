using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Commands.ActiveInActiveFaciltityService;

public record ActiveinActiveFacilityServiceCommand(Guid Id, bool Active)
    : IRequest<ApiResult<bool>>;
public class ActiveinActiveFacilityServiceCommandHandler
    : IRequestHandler<ActiveinActiveFacilityServiceCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public ActiveinActiveFacilityServiceCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        ActiveinActiveFacilityServiceCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.FacilityService>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Service not found.");

        entity.IsActive = request.Active;


        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Service Updated successfully.");
    }
}
