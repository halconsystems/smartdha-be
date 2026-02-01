using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Queries;

public record FacilityUnitBookingConfigDTO(Guid Id, Guid FacilityUnitId, BookingMode BookingMode, bool RequiresApproval, int? SlotDurationMinutes, TimeOnly? OpeningTime, TimeOnly? ClosingTime, decimal BasePrice, int MaxConcurrentBookings, bool? IsActive, bool? IsDeleted);
public record GetAllFacilityUnitBookingConfigQuery() : IRequest<ApiResult<List<FacilityUnitBookingConfigDTO>>>;

public class GetAllFacilityUnitBookingConfigQueryHandler : IRequestHandler<GetAllFacilityUnitBookingConfigQuery, ApiResult<List<FacilityUnitBookingConfigDTO>>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetAllFacilityUnitBookingConfigQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<FacilityUnitBookingConfigDTO>>> Handle(GetAllFacilityUnitBookingConfigQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.FacilityUnitBookingConfig>()
            .OrderBy(x => x.BookingMode)
            .Select(x => new FacilityUnitBookingConfigDTO(x.Id, x.FacilityUnitId, x.BookingMode, x.RequiresApproval, x.SlotDurationMinutes, x.OpeningTime, x.ClosingTime, x.BasePrice, x.MaxConcurrentBookings, x.IsActive, x.IsDeleted))
            .ToListAsync(ct);

        return ApiResult<List<FacilityUnitBookingConfigDTO>>.Ok(list);
    }
}




