using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
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

    public GetNearestVehiclesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NearestVehicleDto>> Handle(GetNearestVehiclesQuery request, CancellationToken ct)
    {
        var vehicles = await _context.SvVehicles
            .Where(v => v.Status == SvVehicleStatus.Available &&
                        v.LastLatitude != null &&
                        v.LastLongitude != null)
            .ToListAsync(ct);

        double PanicLat = (double)request.PanicLatitude;
        double PanicLng = (double)request.PanicLongitude;

        var list = vehicles
            .Select(v =>
            {
                double? distance = null;
                if (v.LastLatitude.HasValue && v.LastLongitude.HasValue)
                {
                    distance = HaversineKm(
                        (double)v.LastLatitude.Value,
                        (double)v.LastLongitude.Value,
                        PanicLat,
                        PanicLng);
                }

                return new NearestVehicleDto
                {
                    VehicleId = v.Id,
                    Name = v.Name,
                    RegistrationNo = v.RegistrationNo,
                    VehicleType = v.VehicleType,
                    MapIconKey = v.MapIconKey,
                    LastLatitude = v.LastLatitude,
                    LastLongitude = v.LastLongitude,
                    DistanceKm = distance
                };
            })
            .OrderBy(x => x.DistanceKm)
            .Take(request.MaxResults)
            .ToList();

        return list;
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

