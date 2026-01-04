using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetNearestVehicles;

public record GetNearestVehiclesQuery(
    double PanicLatitude,
    double PanicLongitude,
    int MaxResults = 5
) : IRequest<List<NearestVehicleDto>>;
public class GetNearestVehiclesQueryHandler
    : IRequestHandler<GetNearestVehiclesQuery, List<NearestVehicleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IGeocodingService _geocodingService;
    private readonly IVehicleLocationStore _vehicleLocationStore;

    public GetNearestVehiclesQueryHandler(IApplicationDbContext context, IGeocodingService geocodingService, IVehicleLocationStore vehicleLocationStore)
    {
        _context = context;
        _geocodingService = geocodingService;
        _vehicleLocationStore = vehicleLocationStore;
    }

    public async Task<List<NearestVehicleDto>> Handle(GetNearestVehiclesQuery request, CancellationToken ct)
    {
        var vehicles = await _context.SvVehicles
    .Where(v => v.Status == SvVehicleStatus.Available && v.IsActive==true)
    .ToListAsync(ct);

        double PanicLat = request.PanicLatitude;
        double PanicLng = request.PanicLongitude;

        // (vehicle, lat, lng, timestamp)
        var vehiclesWithLocation = new List<(SvVehicle Vehicle, double Lat, double Lng, DateTime? Timestamp)>();

        foreach (var vehicle in vehicles)
        {
            var json = await _vehicleLocationStore.GetLocationAsync(vehicle.Id);

            double lat, lng;
            DateTime? ts;

            if (json == null || json.Latitude == 0d || json.Longitude == 0d)
            {
                // fallback to DB
                if (!vehicle.LastLatitude.HasValue || !vehicle.LastLongitude.HasValue)
                    continue; // skip if DB also has no location

                lat = vehicle.LastLatitude.Value;
                lng = vehicle.LastLongitude.Value;
                ts = vehicle.LastLocationAt;
            }
            else
            {
                // use JSON
                lat = json.Latitude;
                lng = json.Longitude;
                ts = json.LastLocationUpdateAt;
            }

            vehiclesWithLocation.Add((vehicle, lat, lng, ts));
        }

        // STEP 1 — Rough Haversine on JSON/DB coordinates
        var roughNearest = vehiclesWithLocation
            .Select(x => new
            {
                x.Vehicle,
                x.Lat,
                x.Lng,
                x.Timestamp,
                RoughDistance = HaversineKm(x.Lat, x.Lng, PanicLat, PanicLng)
            })
            .OrderBy(x => x.RoughDistance)
            .Take(10)
            .ToList();

        // STEP 2 — Now call Google for these 10 only
        var finalList = new List<NearestVehicleDto>();

        foreach (var item in roughNearest)
        {
            var v = item.Vehicle;

            var googleData = await _geocodingService.GetDistanceAndTimeAsync(
                item.Lat,      // ← IMPORTANT: USE JSON OR DB lat
                item.Lng,      // ← IMPORTANT: USE JSON OR DB lng
                PanicLat,
                PanicLng,
                ct);

            if (googleData == null)
                continue;

            finalList.Add(new NearestVehicleDto
            {
                VehicleId = v.Id,
                Name = v.Name,
                RegistrationNo = v.RegistrationNo,
                VehicleType = v.VehicleType,
                MapIconKey = v.MapIconKey,

                // return JSON/DB used location
                LastLatitude = item.Lat,
                LastLongitude = item.Lng,

                DistanceKm = googleData.DistanceKm,
                EstimatedMinutes = googleData.DurationMinutes
            });
        }

        // STEP 3 — Final sorting using real Google distance
        return finalList
            .OrderBy(x => x.DistanceKm)
            .Take(request.MaxResults) // top 5
            .ToList();



    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // km
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double val) => (Math.PI / 180) * val;
}

