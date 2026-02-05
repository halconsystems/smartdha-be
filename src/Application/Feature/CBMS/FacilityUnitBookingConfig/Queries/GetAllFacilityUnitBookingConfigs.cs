using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Commands;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Queries;

public record GetAllFacilityUnitBookingConfigsQuery
    : IRequest<List<FacilityUnitBookingConfigResponse>>;

public class GetAllFacilityUnitBookingConfigsHandler
    : IRequestHandler<
        GetAllFacilityUnitBookingConfigsQuery,
        List<FacilityUnitBookingConfigResponse>>
{
    private readonly ICBMSApplicationDbContext _db;

    public GetAllFacilityUnitBookingConfigsHandler(
        ICBMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<FacilityUnitBookingConfigResponse>> Handle(
        GetAllFacilityUnitBookingConfigsQuery request,
        CancellationToken ct)
    {
        var list = await _db.FacilityUnitBookingConfigs
            .AsNoTracking()
            .Where(x => x.IsDeleted != true)
            .OrderByDescending(x => x.Created) // admin-friendly
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
            .ToListAsync(ct);

        return list;
    }
}






