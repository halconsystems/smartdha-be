using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicleSummary;
public record GetSvVehicleSummaryQuery() : IRequest<SvVehicleSummaryDto>;
public class GetSvVehicleSummaryQueryHandler
    : IRequestHandler<GetSvVehicleSummaryQuery, SvVehicleSummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetSvVehicleSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SvVehicleSummaryDto> Handle(GetSvVehicleSummaryQuery request, CancellationToken ct)
    {
        var totalPoints = await _context.SvPoints
            .CountAsync(p => p.IsActive==true, ct);

        // Only Active + Not Maintenance
        var vehicles = await _context.SvVehicles
            .Where(v => v.IsActive==true && v.Status != SvVehicleStatus.Maintenance)
            .ToListAsync(ct);

        var totalVehicles = vehicles.Count;

        var totalAvailable = vehicles.Count(v => v.Status == SvVehicleStatus.Available);
        var totalBusy = vehicles.Count(v => v.Status == SvVehicleStatus.Busy);
        var totalOffline = vehicles.Count(v => v.Status == SvVehicleStatus.Offline);

        return new SvVehicleSummaryDto
        {
            TotalPoints = totalPoints,
            TotalVehicles = totalVehicles,
            TotalAvailable = totalAvailable,
            TotalBusy = totalBusy,
            TotalOffline = totalOffline
        };
    }
}

