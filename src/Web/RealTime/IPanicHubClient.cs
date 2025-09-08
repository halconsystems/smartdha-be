namespace DHAFacilitationAPIs.Web.RealTime;

public interface IPanicHubClient
{
    // server -> client methods
    Task PanicCreated(Guid panicId);
    Task PanicUpdated(Guid panicId);
    Task LocationUpdated(Guid panicId, Guid locationUpdateId);
    Task SummaryChanged();
}
