using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;
using static DHAFacilitationAPIs.Application.Feature.Panic.PanicDto;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetDashboardSummary;
public record GetDashboardSummaryQuery(DateTime? FromDateTime = null, DateTime? ToDateTime = null) : IRequest<DashboardSummaryDto>;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IApplicationDbContext _ctx;
    public GetDashboardSummaryQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery r, CancellationToken ct)
    {
        var to = r.ToDateTime ?? DateTime.Now;
        var from = r.FromDateTime ?? to.AddDays(-7);

        var baseQ = _ctx.PanicRequests.AsNoTracking()
                      .Where(x => x.Created >= from && x.Created < to);

        var total = await baseQ.CountAsync(ct);
        var resolved = await baseQ.CountAsync(x => x.Status == PanicStatus.Resolved, ct);
        var cancelled = await baseQ.CountAsync(x => x.Status ==PanicStatus.Cancelled, ct);
        var open = await baseQ.CountAsync(x => x.Status == PanicStatus.Created
                                                  || x.Status == PanicStatus.Acknowledged
                                                  || x.Status == PanicStatus.InProgress, ct);

        var last24from = to.AddHours(-24);
        var last24h = await _ctx.PanicRequests.AsNoTracking()
                            .CountAsync(x => x.Created >= last24from && x.Created < to, ct);

        var todayStart = DateTime.UtcNow.Date;
        var today = await _ctx.PanicRequests.AsNoTracking()
                            .CountAsync(x => x.Created >= todayStart && x.Created < todayStart.AddDays(1), ct);

        // SLA metrics
        var ackMins = baseQ
     .Where(x => x.AcknowledgedAt != null)
     .AsEnumerable() // forces client-side after filtering
     .Select(x => (x.AcknowledgedAt!.Value - x.Created).TotalMinutes)
     .DefaultIfEmpty(0)
     .Average();

        var resMins = baseQ
            .Where(x => x.ResolvedAt != null)
            .AsEnumerable()
            .Select(x => (x.ResolvedAt!.Value - x.Created).TotalMinutes)
            .DefaultIfEmpty(0)
            .Average();

        return new DashboardSummaryDto(total, open, resolved, cancelled, last24h, today, ackMins, resMins);
    }
}
