using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IGeocodingService
{
    Task<string?> GetAddressFromLatLngAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default);
}
