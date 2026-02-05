using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.CreateFacilityUnitBookingConfig;
public record CreateFacilityUnitBookingConfigCommand(
    FacilityUnitBookingConfigDto Dto
) : IRequest<ApiResult<Guid>>;
public class CreateFacilityUnitBookingConfigHandler
    : IRequestHandler<CreateFacilityUnitBookingConfigCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _db;

    public CreateFacilityUnitBookingConfigHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<Guid>> Handle(
        CreateFacilityUnitBookingConfigCommand request,
        CancellationToken ct)
    {
        var dto = request.Dto;

        // 🔐 One config per FacilityUnit
        var exists = await _db.FacilityUnitBookingConfigs
            .AnyAsync(x =>
                x.FacilityUnitId == dto.FacilityUnitId &&
                x.IsDeleted != true, ct);

        if (exists)
            throw new ValidationException("Booking config already exists for this unit");

        ValidateSlotRules(dto);

        var entity = new Domain.Entities.CBMS.FacilityUnitBookingConfig
        {
            FacilityUnitId = dto.FacilityUnitId,
            BookingMode = dto.BookingMode,
            RequiresApproval = dto.RequiresApproval,
            SlotDurationMinutes = dto.SlotDurationMinutes,
            OpeningTime = dto.OpeningTime,
            ClosingTime = dto.ClosingTime,
            BasePrice = dto.BasePrice,
            MaxConcurrentBookings = dto.MaxConcurrentBookings,
            UseAvailabilityRules = dto.UseAvailabilityRules
        };

        _db.FacilityUnitBookingConfigs.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id);
    }

    private static void ValidateSlotRules(FacilityUnitBookingConfigDto dto)
    {
        if (dto.BookingMode == BookingMode.SlotBased)
        {
            if (!dto.SlotDurationMinutes.HasValue)
                throw new ValidationException("SlotDurationMinutes is required");

            if (!dto.OpeningTime.HasValue || !dto.ClosingTime.HasValue)
                throw new ValidationException("OpeningTime and ClosingTime are required");

            if (dto.OpeningTime >= dto.ClosingTime)
                throw new ValidationException("OpeningTime must be before ClosingTime");
        }
    }
}

