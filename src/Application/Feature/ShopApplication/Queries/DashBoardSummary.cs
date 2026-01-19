using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.VisualBasic;
using static DHAFacilitationAPIs.Application.Feature.ShopApplication.Queries.OrderDto;

namespace DHAFacilitationAPIs.Application.Feature.ShopApplication.Queries;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly ILaundrySystemDbContext _ctx;
    public GetDashboardSummaryQueryHandler(ILaundrySystemDbContext ctx) => _ctx = ctx;

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery r, CancellationToken ct)
    {
        var to = DateTime.Now;

        var orders = _ctx.Orders.AsNoTracking().Where(x => x.IsActive == true);
        var ordersDispatches = _ctx.OrderDispatches.AsNoTracking().Where(x => x.IsActive == true && orders.Select(x => x.Id).Contains(x.OrdersId));
        var deliiveryDetail = _ctx.DeliveryDetails.AsNoTracking().Where(x => x.IsActive == true && orders.Select(x => x.Id).Contains(x.OrderId));

        var total = await orders.CountAsync(ct);
        var resolved = await orders.CountAsync(x => x.OrderStatus == OrderStatus.Resolved, ct);
        var cancelled = await orders.CountAsync(x => x.OrderStatus == OrderStatus.Cancelled, ct);
        var CompleteAmount = await orders.Where(x => x.OrderStatus == OrderStatus.Resolved).SumAsync(x=> x.CollectedAmount, ct);
        var open = await orders.CountAsync(x => x.OrderStatus == OrderStatus.Confirmed
                                                  || x.OrderStatus == OrderStatus.Acknowledged
                                                  || x.OrderStatus == OrderStatus.InProgress, ct);
        var openAmount = await orders.Where(x => x.OrderStatus == OrderStatus.Confirmed
                                                  || x.OrderStatus == OrderStatus.Acknowledged
                                                  || x.OrderStatus == OrderStatus.InProgress).SumAsync(x => x.CollectedAmount, ct);

        var pickUp = await ordersDispatches.Where(x => x.Status == OrderDispatchStatus.AssignedToRider).CountAsync(ct);
        var pickUpAmount = await ordersDispatches.Where(x => x.Status == OrderDispatchStatus.AssignedToRider).SumAsync(x => x.Orders.CollectedAmount);

        var WashnPress = await ordersDispatches.Where(x => x.Status == OrderDispatchStatus.WashnPressProcess).CountAsync(ct);
        var WashnPressAmount = await ordersDispatches.Where(x => x.Status == OrderDispatchStatus.WashnPressProcess).SumAsync(x => x.Orders.CollectedAmount);

        var onlineAmount = await deliiveryDetail.Where(x => x.PaymentMethod == PaymentMethod.Online).SumAsync(x => x.Total, ct);
        var cashAmount = await deliiveryDetail.Where(x => x.PaymentMethod == PaymentMethod.Cash).SumAsync(x => x.Total, ct);

        var last24from = to.AddHours(-24);
        var last24h = Convert.ToInt32(orders
            .CountAsync(x => x.Created >= last24from && x.Created < to, ct));

        var todayStart = DateTime.UtcNow.Date;
        var today = Convert.ToInt32(orders
            .CountAsync(x => x.Created >= todayStart && x.Created < todayStart.AddDays(1), ct));

        var todayAmount = await orders
    .Where(x => x.Created >= todayStart && x.Created < todayStart.AddDays(1)).SumAsync(x => x.CollectedAmount);

        var overallAmount = await orders
    .SumAsync(x => x.CollectedAmount);


        return new DashboardSummaryDto(total, open, openAmount, resolved, CompleteAmount, cancelled,pickUp,pickUpAmount,WashnPress,WashnPressAmount ,last24h, today, onlineAmount, cashAmount,todayAmount, overallAmount);
    }
}
