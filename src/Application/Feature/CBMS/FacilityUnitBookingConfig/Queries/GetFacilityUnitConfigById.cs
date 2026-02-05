using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnitBookingConfig.Queries;

internal class GetFacilityUnitConfigById
{
}
public record GetFacilityUnitConfigByIdQuery(Guid Id) : IRequest<ApiResult<FacilityUnitBookingConfigDTO>>;

public class GetFacilityUnitConfigByIdQueryHandler : IRequestHandler<GetFacilityUnitConfigByIdQuery, ApiResult<FacilityUnitBookingConfigDTO>>
{
    private readonly ICBMSApplicationDbContext _db;
    public GetFacilityUnitConfigByIdQueryHandler(ICBMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<FacilityUnitBookingConfigDTO>> Handle(GetFacilityUnitConfigByIdQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Domain.Entities.CBMS.FacilityUnitBookingConfig>()
            .OrderBy(x => x.BookingMode)
            .Select(x => new FacilityUnitBookingConfigDTO(x.Id, x.FacilityUnitId, x.BookingMode, x.RequiresApproval, x.SlotDurationMinutes, x.OpeningTime, x.ClosingTime, x.BasePrice, x.MaxConcurrentBookings, x.IsActive, x.IsDeleted))
            .FirstOrDefaultAsync(ct);

        if (list == null) return ApiResult<FacilityUnitBookingConfigDTO>.Fail("Service not found.");


        return ApiResult<FacilityUnitBookingConfigDTO>.Ok(list);
    }
}





