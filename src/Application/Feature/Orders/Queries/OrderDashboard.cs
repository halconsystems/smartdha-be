using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;
using static DHAFacilitationAPIs.Application.Feature.Orders.Queries.OrderDashBoardDTO;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;



public record GetOrderDashboardSummaryQuery(DateTime? FromDateTime = null, DateTime? ToDateTime = null) : IRequest<OrderDashboardSummaryDto>;

public class GetOrderDashboardSummaryQueryHandler : IRequestHandler<GetOrderDashboardSummaryQuery, OrderDashboardSummaryDto>
{
    private readonly ILaundrySystemDbContext _ctx;
    public GetOrderDashboardSummaryQueryHandler(ILaundrySystemDbContext ctx) => _ctx = ctx;

    public async Task<OrderDashboardSummaryDto> Handle(GetOrderDashboardSummaryQuery r, CancellationToken ct)
    {
        var to = r.ToDateTime ?? DateTime.Now;
        var from = r.FromDateTime;


        DateTime? fromdate = r.FromDateTime;
        DateTime? todate = r.ToDateTime;

        var baseQ = _ctx.Orders.AsNoTracking()
                     .Where(x =>
                         (!fromdate.HasValue || x.Created >= fromdate.Value) &&
                         (!todate.HasValue || x.Created < todate.Value)
                     );

        var deliverybaseQ = _ctx.DeliveryDetails.AsNoTracking()
                      .Where(x => baseQ.Select(x => x.Id).Contains(x.OrderId));

        var dispatchbaseQ = _ctx.OrderDispatches.AsNoTracking()
              .Where(x => baseQ.Select(x => x.Id).Contains(x.OrdersId));

        var total = await baseQ.CountAsync(ct);
        var TotalAmount = deliverybaseQ.Sum(x => x.Total);
        var Cancelled = await baseQ.Where(x => x.OrderStatus == OrderStatus.Cancelled).CountAsync(ct);
        var CancelledAmount = deliverybaseQ.Where(x => baseQ.Where(x => x.OrderStatus == OrderStatus.Cancelled).Select(o => o.Id).Contains(x.OrderId)).Sum(x => x.Total);
        var confirmed = await baseQ.Where(x => x.OrderStatus == OrderStatus.Acknowledged).CountAsync(ct);
        var confirmedAmount = deliverybaseQ.Where(x => baseQ.Where(X => X.OrderStatus == OrderStatus.Acknowledged).Select(x => x.Id).Contains(x.OrderId)).Sum(x => x.Total);
        var Pending = await baseQ.Where(x => x.OrderStatus == OrderStatus.Confirmed).CountAsync(ct);
        var PendingAmount = deliverybaseQ.Where(x => baseQ.Where(X => X.OrderStatus == OrderStatus.Confirmed).Select(x => x.Id).Contains(x.OrderId)).Sum(x => x.Total);
        var washing = await dispatchbaseQ.Where(x => x.Status == OrderDispatchStatus.WashnPressProcess).CountAsync(ct);

        var washingOrderIds = dispatchbaseQ
    .Where(x => x.Status == OrderDispatchStatus.WashnPressProcess)
    .Select(x => x.OrdersId);

        var washingAmount = deliverybaseQ.Where(x => washingOrderIds.Contains(x.OrderId)).Sum(x => x.Total);

        var readyForPickup = await dispatchbaseQ.Where(x => x.Status == OrderDispatchStatus.AssignedToRider).CountAsync(ct);

        var readyForPickupIds = dispatchbaseQ
    .Where(x => x.Status == OrderDispatchStatus.AssignedToRider)
    .Select(x => x.OrdersId);

        var readyForPickupAmount = await deliverybaseQ.Where(x => readyForPickupIds.Contains(x.OrderId)).SumAsync(x => x.Total);

        var readyForDeliver = await dispatchbaseQ.Where(x => x.Status == OrderDispatchStatus.ParcelReady).CountAsync(ct);

        var readyForDeliverIds = dispatchbaseQ
    .Where(x => x.Status == OrderDispatchStatus.ParcelReady)
    .Select(x => x.OrdersId);

        var readyForDeliverAmount = deliverybaseQ.Where(x => readyForDeliverIds.Contains(x.OrderId)).Sum(x => x.Total);

        var Delivered = await baseQ.Where(x => x.OrderStatus == OrderStatus.Resolved).CountAsync(ct);

        var DeliveredAmount = deliverybaseQ.Where(x => baseQ.Where(X => X.OrderStatus == OrderStatus.Resolved).Select(x => x.Id).Contains(x.OrderId)).Sum(x => x.Total);


        var OnlineOrder = await deliverybaseQ.Where(x => x.PaymentMethod == PaymentMethod.Online).CountAsync(ct);
        var OnlineOrderAmount = await deliverybaseQ.Where(x => x.PaymentMethod == PaymentMethod.Online).SumAsync(x => x.Total,ct);


        var CashOrders = await deliverybaseQ.Where(x => x.PaymentMethod == PaymentMethod.Cash).CountAsync(ct);
        var CashOrdersAmount = await deliverybaseQ.Where(x => x.PaymentMethod == PaymentMethod.Cash).SumAsync(x => x.Total,ct);


        var last24from = to.AddHours(-24);
        var last24hOrder = _ctx.Orders.AsNoTracking()
                            .Where(x => x.Created >= last24from && x.Created < to);

        var Last24hOrderDelivery = _ctx.DeliveryDetails.AsNoTracking()
                      .Where(x => last24hOrder.Select(x => x.Id).Contains(x.OrderId));


        var Last24h = await last24hOrder.CountAsync(ct);
        var Last24hAmount = Last24hOrderDelivery.Sum(x => x.Total);




        var todayStart = DateTime.UtcNow.Date;
        var today = _ctx.Orders.AsNoTracking()
                            .Where(x => x.Created >= todayStart && x.Created < todayStart.AddDays(1));

        var todayDelivery = _ctx.DeliveryDetails.AsNoTracking()
                     .Where(x => today.Select(x => x.Id).Contains(x.OrderId));

        var Today = await today.CountAsync(ct);
        var TodayAmount = await todayDelivery.SumAsync(x => x.Total);



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

        return new OrderDashboardSummaryDto(total, TotalAmount, confirmed,confirmedAmount, Pending, PendingAmount, washing,washingAmount,readyForPickup,readyForPickupAmount,readyForDeliver,readyForDeliverAmount,Delivered,DeliveredAmount, Cancelled,CancelledAmount, OnlineOrder,OnlineOrderAmount,CashOrders,CashOrdersAmount, Last24h,Last24hAmount, Today, TodayAmount);
    }
}
