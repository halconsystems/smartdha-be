using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IPanicRealtime
{
    Task PanicCreatedAsync(PanicCreatedRealtimeDto dto);
    Task PanicUpdateAsync(PanicCreatedRealtimeDto dto);
    Task SendPanicUpdatedAsync(PanicUpdatedRealtimeDto dto, CancellationToken ct);
    Task UpdateLocationAsync(UpdateLocation dto);
   
}
