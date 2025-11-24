using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicleStatusSummary;
public record GetSvVehicleStatusSummaryQuery() : IRequest<SvVehicleStatusSummaryDto>;

public class GetSvVehicleStatusSummaryQueryHandler
    : IRequestHandler<GetSvVehicleStatusSummaryQuery, SvVehicleStatusSummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetSvVehicleStatusSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SvVehicleStatusSummaryDto> Handle(GetSvVehicleStatusSummaryQuery request, CancellationToken ct)
    {
        var vehicles = await _context.SvVehicles
            .Where(v => v.IsActive==true)
            .ToListAsync(ct);

        var summary = new SvVehicleStatusSummaryDto
        {
            Total = vehicles.Count,

            Offline = vehicles.Count(v => v.Status == SvVehicleStatus.Offline),
            Available = vehicles.Count(v => v.Status == SvVehicleStatus.Available),
            Busy = vehicles.Count(v => v.Status == SvVehicleStatus.Busy),
            Maintenance = vehicles.Count(v => v.Status == SvVehicleStatus.Maintenance)
        };

        return summary;
    }
}

