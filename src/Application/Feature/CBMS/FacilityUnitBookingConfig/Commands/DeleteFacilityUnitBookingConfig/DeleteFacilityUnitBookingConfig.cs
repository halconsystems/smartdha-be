using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.DeleteFacilityUnitBookingConfig;

public record DeleteFacilityUnitBookingConfigCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteFacilityUnitBookingConfigCommandHandler
    : IRequestHandler<DeleteFacilityUnitBookingConfigCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public DeleteFacilityUnitBookingConfigCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteFacilityUnitBookingConfigCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.FacilityUnitBookingConfig>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Facility Unit Booking Config Not Found");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Facility Unit Booking Config deleted successfully.");
    }
}

