using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;

public interface IOrderRealTime
{
    Task SendOrderUpdatedAsync(OrderUpdateRealTimeDTO dto, CancellationToken ct);
    Task OrderCreatedAsync(OrderCreatedRealtimeDto dto);
    Task OrderUpdateAsync(OrderCreatedRealtimeDto dto);
    Task UpdateLocationAsync(UpdateLocation dto);
}
