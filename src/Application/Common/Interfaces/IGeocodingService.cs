using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IGeocodingService
{
    Task<string?> GetAddressFromLatLngAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default);

    Task<GoogleRouteResult?> GetDistanceAndTimeAsync(
        double originLat, double originLng,
        double destLat, double destLng,
        CancellationToken ct);
}
