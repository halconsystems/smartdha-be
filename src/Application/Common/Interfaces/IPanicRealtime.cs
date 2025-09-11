using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.Panic;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IPanicRealtime
{
    Task PanicCreatedAsync(PanicCreatedRealtimeDto dto);
}
