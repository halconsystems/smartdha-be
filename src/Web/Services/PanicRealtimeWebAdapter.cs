using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Web.RealTime;
using Microsoft.AspNetCore.SignalR;

namespace DHAFacilitationAPIs.Web.Services;

public class PanicRealtimeWebAdapter : IPanicRealtime
{
    private readonly IHubContext<PanicHub, IPanicHubClient> _hub;
    public PanicRealtimeWebAdapter(IHubContext<PanicHub, IPanicHubClient> hub) => _hub = hub;

    public async Task PanicCreatedAsync(PanicCreatedRealtimeDto panicId)
    {
        await _hub.Clients.Group(PanicHub.PanicGroups.Dispatchers).PanicCreated(panicId);
       // await _hub.Clients.All.SummaryChanged();
    }

    public async Task PanicUpdateAsync(PanicCreatedRealtimeDto dto)
    {
        await _hub.Clients.Group(PanicHub.PanicGroups.Dispatchers).PanicUpdated(dto);
    }

    public async Task SendPanicUpdatedAsync(PanicUpdatedRealtimeDto dto, CancellationToken ct)
    {
        await _hub.Clients.Group(PanicHub.PanicGroups.Dispatchers).SendPanicUpdatedAsync(dto);
    }

    public async Task UpdateLocationAsync(UpdateLocation dto)
    {
        await _hub.Clients.Group(PanicHub.PanicGroups.Dispatchers).VehicleLocationUpdate(dto);
    }
}
