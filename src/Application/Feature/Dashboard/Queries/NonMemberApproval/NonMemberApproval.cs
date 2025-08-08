using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Dashboard.Queries.NonMemberApproval;
public record GetNonMemberVerificationDashboardQuery()
    : IRequest<SuccessResponse<NonMemberVerificationDashboardDto>>;

public class GetNonMemberVerificationDashboardQueryHandler
    : IRequestHandler<GetNonMemberVerificationDashboardQuery, SuccessResponse<NonMemberVerificationDashboardDto>>
{
    private readonly IApplicationDbContext _ctx;

    public GetNonMemberVerificationDashboardQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<NonMemberVerificationDashboardDto>> Handle(GetNonMemberVerificationDashboardQuery request, CancellationToken ct)
    {
        // base query
        var q = _ctx.NonMemberVerifications
            .AsNoTracking()
            .Where(x => x.IsDeleted == false || x.IsDeleted == null);

        // ---- Summary in ONE query (no concurrency) ----
        var summaryRaw = await q
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Approved = g.Count(x => x.Status == VerificationStatus.Approved),
                Rejected = g.Count(x => x.Status == VerificationStatus.Rejected),
                Pending = g.Count(x => x.Status == VerificationStatus.Pending)
            })
            .SingleOrDefaultAsync(ct);

        var summary = new NonMemberVerificationSummaryDto(
            TotalRequests: summaryRaw?.Total ?? 0,
            Approved: summaryRaw?.Approved ?? 0,
            Rejected: summaryRaw?.Rejected ?? 0,
            Pending: summaryRaw?.Pending ?? 0
        );

        // ---- Current month day-wise (Created date) ----
        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfNext = startOfMonth.AddMonths(1);

        var dailyRaw = await q
            .Where(x => x.Created >= startOfMonth && x.Created < startOfNext)
            .GroupBy(x => new { x.Created.Year, x.Created.Month, x.Created.Day })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.Day,
                Requests = g.Count(),
                Approved = g.Count(n => n.Status == VerificationStatus.Approved),
                Rejected = g.Count(n => n.Status == VerificationStatus.Rejected)
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
            .ToListAsync(ct);

        var daily = dailyRaw.Select(x => new NonMemberVerificationDayWiseDto(
            Date: new DateTime(x.Year, x.Month, x.Day, 0, 0, 0, DateTimeKind.Utc).ToString("yyyy-MM-dd"),
            Requests: x.Requests,
            Approved: x.Approved,
            Rejected: x.Rejected
        )).ToList();

        var payload = new NonMemberVerificationDashboardDto(summary, daily);
        return Success.Ok(payload, "Dashboard loaded.");
    }
}


