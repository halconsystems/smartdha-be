using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Web.RealTime;

public interface IOrderHubClient
{
    Task OrderCreated(OrderCreatedRealtimeDto orderId);
    Task OrderUpdated(OrderCreatedRealtimeDto orderId);
    Task SendOrderUpdatedAsync(OrderUpdateRealTimeDTO dto);
    Task VehicleLocationUpdate(UpdateLocation dto);
}
