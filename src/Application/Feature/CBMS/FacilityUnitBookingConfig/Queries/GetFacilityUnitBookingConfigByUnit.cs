using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands;
using DHAFacilitationAPIs.Domain.Enums.CBMS;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Queries;

public record GetFacilityUnitBookingConfigByUnitQuery(Guid FacilityUnitId)
    : IRequest<FacilityUnitBookingConfigResponse>;

public class GetFacilityUnitBookingConfigByUnitHandler
    : IRequestHandler<GetFacilityUnitBookingConfigByUnitQuery, FacilityUnitBookingConfigResponse>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetFacilityUnitBookingConfigByUnitHandler(ICBMSApplicationDbContext db)
        => _db = db;

    public async Task<FacilityUnitBookingConfigResponse> Handle(
        GetFacilityUnitBookingConfigByUnitQuery request,
        CancellationToken ct)
    {
        var config = await _db.FacilityUnitBookingConfigs
            .AsNoTracking()
            .Where(x =>
                x.FacilityUnitId == request.FacilityUnitId &&
                x.IsDeleted != true)
            .Select(x => new FacilityUnitBookingConfigResponse(
                x.Id,
                x.FacilityUnitId,
                x.BookingMode,
                x.RequiresApproval,
                x.SlotDurationMinutes,
                x.OpeningTime,
                x.ClosingTime,
                x.BasePrice,
                x.MaxConcurrentBookings,
                x.UseAvailabilityRules
            ))
            .FirstOrDefaultAsync(ct);

        if (config == null)
            throw new NotFoundException("Booking config not found");

        return config;
    }
}




