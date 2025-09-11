using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Panic;
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
        await _hub.Clients.All.SummaryChanged();
    }

    public async Task PanicUpdatedAsync(Guid panicId)
    {
        await _hub.Clients.Group(PanicHub.PanicGroups.Dispatchers).PanicUpdated(panicId);
        await _hub.Clients.All.SummaryChanged();
    }

    public Task LocationUpdatedAsync(Guid panicId, Guid locationUpdateId)
        => _hub.Clients.Group(PanicHub.PanicGroups.Panic(panicId)).LocationUpdated(panicId, locationUpdateId);

    public Task SummaryChangedAsync()
        => _hub.Clients.All.SummaryChanged();
}
