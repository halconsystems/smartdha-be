using DHAFacilitationAPIs.Application.Feature.Panic;

namespace DHAFacilitationAPIs.Web.RealTime;

public interface IPanicHubClient
{
    // server -> client methods
    Task PanicCreated(PanicCreatedRealtimeDto panicId);
    Task PanicUpdated(Guid panicId);
    Task LocationUpdated(Guid panicId, Guid locationUpdateId);
    Task SummaryChanged();
}
