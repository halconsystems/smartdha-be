using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using static DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationDashboard.Queries.FemugationDashBoardDTO;
using static DHAFacilitationAPIs.Application.Feature.Orders.Queries.OrderDashBoardDTO;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationDashboard.Queries;


public record GetFemugationDashboardSummaryQuery(DateTime? FromDateTime = null, DateTime? ToDateTime = null) : IRequest<FemugationDashboardSummaryDto>;

public class GetFemugationDashboardSummaryQueryHandler : IRequestHandler<GetFemugationDashboardSummaryQuery, FemugationDashboardSummaryDto>
{
    private readonly IApplicationDbContext _ctx;
    public GetFemugationDashboardSummaryQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<FemugationDashboardSummaryDto> Handle(GetFemugationDashboardSummaryQuery r, CancellationToken ct)
    {
        var to = r.ToDateTime ?? DateTime.Now;
        var from = r.FromDateTime;

        var baseQ = _ctx.Fumigations.AsNoTracking()
                      .Where(x => x.Created >= from && x.Created < to);

        //var deliverybaseQ = _ctx.DeliveryDetails.AsNoTracking()
        //              .Where(x => baseQ.Select(x => x.Id).Contains(x.OrderId));

        //var dispatchbaseQ = _ctx.OrderDispatches.AsNoTracking()
        //      .Where(x => baseQ.Select(x => x.Id).Contains(x.OrdersId));

        var total = await baseQ.CountAsync(ct);
        var TotalAmount = baseQ.Sum(x => x.Total);
        var Cancelled = await baseQ.Where(x => x.FemStatus == FemStatus.Cancelled).CountAsync(ct);
        var CancelledAmount = await baseQ.Where(x => x.FemStatus == FemStatus.Cancelled).SumAsync(x => x.Total,ct);
        var Acknowledged = await baseQ.Where(x => x.FemStatus == FemStatus.Acknowledged).CountAsync(ct);
        var AcknowledgedAmount = await baseQ.Where(x => x.FemStatus == FemStatus.Acknowledged).SumAsync(x => x.Total,ct);
        var Pending = await baseQ.Where(x => x.FemStatus == FemStatus.InProgress).CountAsync(ct);
        var PendingAmount = await baseQ.Where(x => x.FemStatus == FemStatus.InProgress).SumAsync(x => x.Total,ct);

        var resolved = await baseQ.Where(x => x.FemStatus == FemStatus.Resolved).CountAsync(ct);
        var resolvedAmount = await baseQ.Where(x => x.FemStatus == FemStatus.Resolved).SumAsync(x => x.Total, ct);

        var OnlineOrder = await baseQ.Where(x => x.PaymentMethod == PaymentMethod.Online).CountAsync(ct);
        var OnlineOrderAmount = await baseQ.Where(x => x.PaymentMethod == PaymentMethod.Online).SumAsync(x => x.Total, ct);

        var CashOrders = await baseQ.Where(x => x.PaymentMethod == PaymentMethod.Cash).CountAsync(ct);
        var CashOrdersAmount = await baseQ.Where(x => x.PaymentMethod == PaymentMethod.Cash).SumAsync(x => x.Total, ct);


        var last24from = to.AddHours(-24);
        var last24hOrder = _ctx.Fumigations.AsNoTracking()
                            .Where(x => x.Created >= last24from && x.Created < to);

        var Last24h = await last24hOrder.CountAsync(ct);
        var Last24hAmount = last24hOrder.Sum(x => x.Total);




        var todayStart = DateTime.UtcNow.Date;
        var today = _ctx.Fumigations.AsNoTracking()
                            .Where(x => x.Created >= todayStart && x.Created < todayStart.AddDays(1));


        var Today = await today.CountAsync(ct);
        var TodayAmount = await today.SumAsync(x => x.Total);



        //   // SLA metrics
        //   var ackMins = baseQ
        //.Where(x => x.AcknowledgedAt != null)
        //.AsEnumerable() // forces client-side after filtering
        //.Select(x => (x.AcknowledgedAt!.Value - x.Created).TotalMinutes)
        //.DefaultIfEmpty(0)
        //.Average();

        //   var resMins = baseQ
        //       .Where(x => x.ResolvedAt != null)
        //       .AsEnumerable()
        //       .Select(x => (x.ResolvedAt!.Value - x.Created).TotalMinutes)
        //       .DefaultIfEmpty(0)
        //       .Average();

        return new FemugationDashboardSummaryDto(total, TotalAmount, resolved, resolvedAmount, Pending, PendingAmount, Acknowledged ,AcknowledgedAmount, Cancelled, CancelledAmount, OnlineOrder, OnlineOrderAmount, CashOrders, CashOrdersAmount, Last24h, Last24hAmount, Today, TodayAmount);
    }
}
