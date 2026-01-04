using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Web.RealTime;

public interface IPanicHubClient
{
    // server -> client methods
    Task PanicCreated(PanicCreatedRealtimeDto panicId);
    Task PanicUpdated(PanicCreatedRealtimeDto panicId);
    Task SendPanicUpdatedAsync(PanicUpdatedRealtimeDto dto);
    Task VehicleLocationUpdate(UpdateLocation dto);
}
