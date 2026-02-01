using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitService.Commands.DeleteFaciltityUnitService;

internal class DeleteFacilityUnitService
{
}
public record DeleteFacilityUnitServiceCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteFacilityUnitServiceCommandHandler
    : IRequestHandler<DeleteFacilityUnitServiceCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public DeleteFacilityUnitServiceCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteFacilityUnitServiceCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.FacilityUnitService>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Facility Unit Service Not Found");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Facility Unit Service deleted successfully.");
    }
}

