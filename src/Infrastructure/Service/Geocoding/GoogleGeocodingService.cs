using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Service.Geocoding;
public class GoogleGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsOptions _options;
    private readonly IApplicationDbContext _context;

    public GoogleGeocodingService(
        HttpClient httpClient,
        IOptions<GoogleMapsOptions> options,
        IApplicationDbContext context)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _context = context;
    }

    public async Task<string?> GetAddressFromLatLngAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        var log = new GoogleApiLog
        {
            Latitude = latitude,
            Longitude = longitude,
            Created = DateTime.UtcNow
        };

        try
        {
            var url =
            $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={_options.ApiKey}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string status = root.GetProperty("status").GetString() ?? "UNKNOWN";
            log.ResponseStatus = status;

            if (!status.Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                log.IsSuccess = false;
                log.ErrorMessage = $"Google API returned status: {status}";
                await SaveLogAsync(log, cancellationToken);
                return null;
            }

            var address = root
                .GetProperty("results")[0]
                .GetProperty("formatted_address")
                .GetString();

            log.IsSuccess = true;
            log.FormattedAddress = address;

            await SaveLogAsync(log, cancellationToken);

            return address;
        }
        catch (Exception ex)
        {
            log.IsSuccess = false;
            log.ErrorMessage = ex.Message;

            await SaveLogAsync(log, cancellationToken);
            return null;
        }
    }

    private async Task SaveLogAsync(GoogleApiLog log, CancellationToken ct)
    {
        _context.GoogleApiLogs.Add(log);
        await _context.SaveChangesAsync(ct);
    }
}
