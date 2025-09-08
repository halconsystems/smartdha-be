using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IPanicRealtime
{
    Task PanicCreatedAsync(Guid id);
    Task PanicUpdatedAsync(Guid id);
    Task LocationUpdatedAsync(Guid panicRequestId, Guid locationUpdateId);
    Task SummaryChangedAsync();
}
