using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Service.Geocoding;
public class GoogleGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsOptions _options;

    public GoogleGeocodingService(
        HttpClient httpClient,
        IOptions<GoogleMapsOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string?> GetAddressFromLatLngAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default)
    {
        var url =
            $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={_options.ApiKey}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("status", out var statusProp))
            return null;

        var status = statusProp.GetString();
        if (!string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase))
            return null;

        if (!root.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
            return null;

        var formattedAddress = results[0]
            .GetProperty("formatted_address")
            .GetString();

        return formattedAddress;
    }
}
