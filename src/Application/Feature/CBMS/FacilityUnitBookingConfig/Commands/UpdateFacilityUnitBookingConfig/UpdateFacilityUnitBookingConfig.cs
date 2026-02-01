using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.CreateFacilityUnitBookingConfig;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.UpdateFacilityUnitBookingConfig;


public record UpdateFacilityUnitBookingConfigCommand(
    Guid Id,
    CreateFacilityUnitBookingConfigDto Dto
) : IRequest<ApiResult<Guid>>;
public class UpdateFacilityUnitBookingConfigCommandHandler
    : IRequestHandler<UpdateFacilityUnitBookingConfigCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public UpdateFacilityUnitBookingConfigCommandHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        UpdateFacilityUnitBookingConfigCommand request,
        CancellationToken ct)
    {
        var existFacilityUnitConfig = await _db.FacilityUnitBookingConfigs
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (existFacilityUnitConfig == null)
            return ApiResult<Guid>.Fail("Facility Unit Booking Config Not Found");

        existFacilityUnitConfig.FacilityUnitId = request.Dto.FacilityUnitId;
        existFacilityUnitConfig.BookingMode = request.Dto.BookingMode;
        existFacilityUnitConfig.BasePrice = request.Dto.BasePrice;
        existFacilityUnitConfig.RequiresApproval = request.Dto.RequiresApproval;
        existFacilityUnitConfig.SlotDurationMinutes = request.Dto.SlotDurationMinutes;
        existFacilityUnitConfig.OpeningTime = request.Dto.OpeningTime;
        existFacilityUnitConfig.ClosingTime = request.Dto.ClosingTime;

        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(existFacilityUnitConfig.Id, "Booking config Updated");
    }
}


