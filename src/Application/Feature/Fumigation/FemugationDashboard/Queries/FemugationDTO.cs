using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemugationDashboard.Queries;


public class FemugationDashBoardDTO
{
    public record FemugationDashboardSummaryDto(
    int Total,
    decimal? TotalAmount,
    int resolved,
    decimal? resolvedAmount,
    int Pending,
    decimal? pendingAmount,
    int Acknowledged,
    decimal? AcknowledgedAmount,
    int Cancelled,
    decimal? CancelledAmount,
    int OnlineOrder,
    decimal? OnlineOrderAmount,
    int CashOrders,
    decimal? CashOrdersAmount,
    int Last24h,
    decimal? Last24hAmount,
    int Today,
    decimal? TodayAmount
    //double? deilveryRate,
    //double? inprogessRate,
    //double? ReaslyForDeilverRate,
    //double? WashingRate,
    //double? pickupRate
);
}
