using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Dashboard.Queries.NonMemberApproval;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

public record MemberShipDashboardDTO
(
    int Requests,
    int Approved,
    int Rejected,
    int Pending,
    decimal ApprovalRate,
    decimal RejectedRate
    );

public record MemberVerificationDayWiseDto(
    string Date,   // "yyyy-MM-dd" for easy charting
    int Requests,
    int Approved,
    int Rejected,
    int Pending,
    decimal ApprovalRate,
    decimal RejectedRate
);


public record MemberVerificationDashboardDto(
    MemberShipDashboardDTO Summary,
    List<MemberVerificationDayWiseDto> CurrentMonthDaily
);
public record GetMemberShipDashboardSummaryQuery(DateTime? FromDateTime = null, DateTime? ToDateTime = null) : IRequest<MemberVerificationDashboardDto>;

public class GetMemberShipDashboardSummaryQueryHandler : IRequestHandler<GetMemberShipDashboardSummaryQuery, MemberVerificationDashboardDto>
{
    private readonly IApplicationDbContext _ctx;
    public GetMemberShipDashboardSummaryQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<MemberVerificationDashboardDto> Handle(GetMemberShipDashboardSummaryQuery r, CancellationToken ct)
    {
        var to = r.ToDateTime ?? DateTime.Now;
        var from = r.FromDateTime ?? to.AddDays(-7);


        var q = _ctx.MemberRequests
            .AsNoTracking()
            .Where(x => x.Created >= from && x.Created < to && (x.IsDeleted == false || x.IsDeleted == null));

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



        var summary = new MemberShipDashboardDTO(
           Requests: summaryRaw?.Total ?? 0,
           Approved: summaryRaw?.Approved ?? 0,
           Rejected: summaryRaw?.Rejected ?? 0,
           Pending: summaryRaw?.Pending ?? 0,
           ApprovalRate: Convert.ToDecimal(summaryRaw?.Total > 0 ? (summaryRaw?.Approved / summaryRaw?.Total) : 0),
           RejectedRate: Convert.ToDecimal(summaryRaw?.Total > 0 ? (summaryRaw?.Rejected / summaryRaw?.Total) : 0)
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
                Rejected = g.Count(n => n.Status == VerificationStatus.Rejected),
                ApprovalRate = g.Count() > 0 ? (g.Count(n => n.Status == VerificationStatus.Approved)/ g.Count()) : 0,
                RejectedRate = g.Count() > 0 ? (g.Count(n => n.Status == VerificationStatus.Rejected) / g.Count()) : 0
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
            .ToListAsync(ct);

        var daily = dailyRaw.Select(x => new MemberVerificationDayWiseDto(
            Date: new DateTime(x.Year, x.Month, x.Day, 0, 0, 0, DateTimeKind.Utc).ToString("yyyy-MM-dd"),
            Requests: x.Requests,
            Approved: x.Approved,
            Rejected: x.Rejected,
            Pending: Convert.ToInt32(x.Requests) - Convert.ToInt32(x.Approved),
            ApprovalRate: Convert.ToDecimal(x.Requests) > 0 ? (Convert.ToDecimal(x.Approved)/ Convert.ToDecimal(x.Requests)) : 0,
            RejectedRate: Convert.ToDecimal(x.Requests) > 0 ? (Convert.ToDecimal(x.Rejected) / Convert.ToDecimal(x.Requests)) : 0
        )).ToList();


        return new MemberVerificationDashboardDto(summary, daily);
    }
}
