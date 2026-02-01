using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.ActiveInActiveFacilityUnitBookingConfig;

public record ActiveInActiveFacilityUnitBookingConfigCommand(Guid Id, bool Active)
    : IRequest<ApiResult<bool>>;
public class ActiveInActiveFacilityUnitBookingConfigCommandHandler
    : IRequestHandler<ActiveInActiveFacilityUnitBookingConfigCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public ActiveInActiveFacilityUnitBookingConfigCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        ActiveInActiveFacilityUnitBookingConfigCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.FacilityUnitBookingConfig>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Facility Unit Booking Config not found.");

        entity.IsActive = request.Active;


        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Facility Unit Booking Config Updated successfully.");
    }
}
