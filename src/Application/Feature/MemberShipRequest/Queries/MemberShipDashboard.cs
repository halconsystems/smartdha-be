using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

public record MemberShipDashboardDTO
(
    int total,
    int approved,
    int rejected,
    int pending,
    decimal ApprovalRate,
    decimal rejectedRate
    );
public record GetMemberShipDashboardSummaryQuery(DateTime? FromDateTime = null, DateTime? ToDateTime = null) : IRequest<MemberShipDashboardDTO>;

public class GetMemberShipDashboardSummaryQueryHandler : IRequestHandler<GetMemberShipDashboardSummaryQuery, MemberShipDashboardDTO>
{
    private readonly IApplicationDbContext _ctx;
    public GetMemberShipDashboardSummaryQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<MemberShipDashboardDTO> Handle(GetMemberShipDashboardSummaryQuery r, CancellationToken ct)
    {
        var to = r.ToDateTime ?? DateTime.Now;
        var from = r.FromDateTime ?? to.AddDays(-7);

        var baseQ = _ctx.MemberRequests.AsNoTracking()
                      .Where(x => x.Created >= from && x.Created < to);

        //var deliverybaseQ = _ctx.DeliveryDetails.AsNoTracking()
        //              .Where(x => baseQ.Select(x => x.Id).Contains(x.OrderId));

        //var dispatchbaseQ = _ctx.OrderDispatches.AsNoTracking()
        //      .Where(x => baseQ.Select(x => x.Id).Contains(x.OrdersId));
        var total = await baseQ.CountAsync(ct);

        var rejected = await baseQ
            .Where(x => x.Status == Domain.Enums.VerificationStatus.Rejected)
            .CountAsync(ct);

        var pending = await baseQ
            .Where(x => x.Status == Domain.Enums.VerificationStatus.Pending)
            .CountAsync(ct);

        var approved = await baseQ
            .Where(x => x.Status == Domain.Enums.VerificationStatus.Approved)
            .CountAsync(ct);

        decimal approvalRate = 0;
        decimal rejectionRate = 0;

        if (total > 0)
        {
            approvalRate = (decimal)approved / total * 100;
            rejectionRate = (decimal)rejected / total * 100;
        }


        return new MemberShipDashboardDTO(total, approved, rejected, pending, approvalRate, rejectionRate);
    }
}
