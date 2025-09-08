namespace MobileAPI.RealTime;

public interface IPanicHubClient
{
    Task PanicCreated(Guid panicId);
    Task PanicUpdated(Guid panicId);
    Task LocationUpdated(Guid panicId, Guid locationUpdateId);
    Task SummaryChanged();
}
