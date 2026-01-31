using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.CreateFacilityUnitBookingConfig;
public record CreateFacilityUnitBookingConfigCommand(
    CreateFacilityUnitBookingConfigDto Dto
) : IRequest<ApiResult<Guid>>;
public class CreateFacilityUnitBookingConfigHandler
    : IRequestHandler<CreateFacilityUnitBookingConfigCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public CreateFacilityUnitBookingConfigHandler(ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateFacilityUnitBookingConfigCommand request,
        CancellationToken ct)
    {
        var exists = await _db.FacilityUnitBookingConfigs
            .AnyAsync(x => x.FacilityUnitId == request.Dto.FacilityUnitId, ct);

        if (exists)
            return ApiResult<Guid>.Fail("Booking config already exists");

        var config = new DHAFacilitationAPIs.Domain.Entities.CBMS.FacilityUnitBookingConfig
        {
            FacilityUnitId = request.Dto.FacilityUnitId,
            BookingMode = request.Dto.BookingMode,
            BasePrice = request.Dto.BasePrice,
            RequiresApproval = request.Dto.RequiresApproval,
            SlotDurationMinutes = request.Dto.SlotDurationMinutes,
            OpeningTime = request.Dto.OpeningTime,
            ClosingTime = request.Dto.ClosingTime
        };

        _db.FacilityUnitBookingConfigs.Add(config);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(config.Id, "Booking config created");
    }
}

