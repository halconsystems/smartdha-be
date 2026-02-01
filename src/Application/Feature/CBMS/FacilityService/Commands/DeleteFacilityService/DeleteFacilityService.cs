using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Commands.DeleteFacilityService;

public record DeleteFacilityServiceCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteFacilityServiceCommandHandler
    : IRequestHandler<DeleteFacilityServiceCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public DeleteFacilityServiceCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteFacilityServiceCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.FacilityService>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Service not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Service deleted successfully.");
    }
}

