using DHAFacilitationAPIs.Application.Feature.Panic;

namespace MobileAPI.RealTime;

public interface IPanicHubClient
{
    Task PanicCreated(PanicCreatedRealtimeDto dto);
    Task PanicUpdated(Guid panicId);
    Task LocationUpdated(Guid panicId, Guid locationUpdateId);
    Task SummaryChanged();
}
