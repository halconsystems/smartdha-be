using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.CreateFacilityUnitBookingConfig;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands.UpdateFacilityUnitBookingConfig;


public record UpdateFacilityUnitBookingConfigCommand(
    Guid Id,
    FacilityUnitBookingConfigDto Dto
) : IRequest<ApiResult<bool>>;

public class UpdateFacilityUnitBookingConfigHandler
    : IRequestHandler<UpdateFacilityUnitBookingConfigCommand, ApiResult<bool>>
{
    private readonly ICBMSApplicationDbContext _db;

    public UpdateFacilityUnitBookingConfigHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<ApiResult<bool>> Handle(
        UpdateFacilityUnitBookingConfigCommand request,
        CancellationToken ct)
    {
        var entity = await _db.FacilityUnitBookingConfigs
            .FirstOrDefaultAsync(x =>
                x.Id == request.Id &&
                x.IsDeleted != true, ct);

        if (entity == null)
            throw new DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException("Booking config not found");

        ValidateSlotRules(request.Dto);

        var dto = request.Dto;

        entity.BookingMode = dto.BookingMode;
        entity.RequiresApproval = dto.RequiresApproval;
        entity.SlotDurationMinutes = dto.SlotDurationMinutes;
        entity.OpeningTime = dto.OpeningTime;
        entity.ClosingTime = dto.ClosingTime;
        entity.BasePrice = dto.BasePrice;
        entity.MaxConcurrentBookings = dto.MaxConcurrentBookings;
        entity.UseAvailabilityRules = dto.UseAvailabilityRules;

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true);
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



