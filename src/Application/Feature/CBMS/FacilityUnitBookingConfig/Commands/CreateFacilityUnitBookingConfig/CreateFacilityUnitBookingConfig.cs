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
public class CreateFacilityUnitBookingConfigCommand
    : IRequest<ApiResult<Guid>>
{
    public Guid FacilityUnitId { get; set; }
    public BookingMode BookingMode { get; set; }
    public decimal BasePrice { get; set; }
    public bool RequiresApproval { get; set; }
    public int? SlotDurationMinutes { get; set; }
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
}


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
            .AnyAsync(x => x.FacilityUnitId == request.FacilityUnitId, ct);

        if (exists)
            return ApiResult<Guid>.Fail("Booking config already exists");

        var config = new DHAFacilitationAPIs.Domain.Entities.CBMS.FacilityUnitBookingConfig
        {
            FacilityUnitId = request.FacilityUnitId,
            BookingMode = request.BookingMode,
            BasePrice = request.BasePrice,
            RequiresApproval = request.RequiresApproval,
            SlotDurationMinutes = request.SlotDurationMinutes,
            OpeningTime = request.OpeningTime,
            ClosingTime = request.ClosingTime
        };

        _db.FacilityUnitBookingConfigs.Add(config);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(config.Id, "Booking config created");
    }
}

