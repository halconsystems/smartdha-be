using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ShopApplication.Queries;

public class OrderDto
{
    public record DashboardSummaryDto(
    int Total,
    int Open,
    decimal? InProgessAmount,// Created + Acknowledged + InProgress
    int Completed,
    decimal? CompleteAmount,
    int Cancelled,    
    int PickupCount,
    decimal? PickUpAmount,
    int WashnPress,
    decimal? WashnPressAmount,

    int Last24h,
    int Today,
    decimal? OnlineAmount,
    decimal? CashAmount,
    decimal? TodayAmount,
    decimal? OverAllAmount
);

}
