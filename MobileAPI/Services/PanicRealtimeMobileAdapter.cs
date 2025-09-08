using DHAFacilitationAPIs.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using MobileAPI.RealTime;

namespace MobileAPI.Services;

public class PanicRealtimeMobileAdapter : IPanicRealtime
{
    private readonly IHubContext<PanicHub, IPanicHubClient> _hub;
    public PanicRealtimeMobileAdapter(IHubContext<PanicHub, IPanicHubClient> hub) => _hub = hub;

    public async Task PanicCreatedAsync(Guid panicId)
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
