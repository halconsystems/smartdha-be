using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;

public class OrderDashBoardDTO
{
    public record OrderDashboardSummaryDto(
    int Total,
    decimal TotalAmount,
    int confirmed,
    decimal confirmedAmount,
    int Pending,
    decimal pendingAmount,
    int washing,
    decimal washingAmount,
     int readyForPickup,
    decimal readyForPickupAmount,
      int readyForDeliver,
    decimal readyForDeliverAmount,
    int Delivered,
    decimal DeliveredAmount,
    int Cancelled,
    decimal CancelledAmount,
    int OnlineOrder,
    decimal OnlineOrderAmount,
    int CashOrders,
    decimal CashOrdersAmount,
    int Last24h,
    decimal Last24hAmount,
    int Today,
    decimal TodayAmount
    //double? deilveryRate,
    //double? inprogessRate,
    //double? ReaslyForDeilverRate,
    //double? WashingRate,
    //double? pickupRate
);
}
