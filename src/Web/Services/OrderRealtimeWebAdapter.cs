using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Web.RealTime;
using Microsoft.AspNetCore.SignalR;

namespace DHAFacilitationAPIs.Web.Services;


public class OrderRealtimeWebAdapter : IOrderRealTime
{
    private readonly IHubContext<OrderHub, IOrderHubClient> _hub;
    public OrderRealtimeWebAdapter(IHubContext<OrderHub, IOrderHubClient> hub) => _hub = hub;

    public async Task OrderCreatedAsync(OrderCreatedRealtimeDto panicId)
    {
        await _hub.Clients.Group(OrderHub.OrdercGroups.Dispatchers).OrderCreated(panicId);
        // await _hub.Clients.All.SummaryChanged();
    }

    public async Task OrderUpdateAsync(OrderCreatedRealtimeDto dto)
    {
        await _hub.Clients.Group(OrderHub.OrdercGroups.Dispatchers).OrderUpdated(dto);
    }

    public async Task SendOrderUpdatedAsync(OrderUpdateRealTimeDTO dto, CancellationToken ct)
    {
        await _hub.Clients.Group(OrderHub.OrdercGroups.Dispatchers).SendOrderUpdatedAsync(dto);
    }

    public async Task UpdateLocationAsync(UpdateLocation dto)
    {
        await _hub.Clients.Group(OrderHub.OrdercGroups.Dispatchers).VehicleLocationUpdate(dto);
    }
}
