using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvMapPoints;
public record GetSvMapPointsQuery() : IRequest<List<SvMapPointDto>>;
public class GetSvMapPointsQueryHandler
    : IRequestHandler<GetSvMapPointsQuery, List<SvMapPointDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSvMapPointsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SvMapPointDto>> Handle(GetSvMapPointsQuery request, CancellationToken ct)
    {
        var points = await _context.SvPoints
            .Include(p => p.Vehicles)
            .Where(p => p.IsActive==true)
            .ToListAsync(ct);

        var result = points.Select(p => new SvMapPointDto
        {
            PointId = p.Id,
            Code = p.Code,
            Name = p.Name,
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            Address = p.Address,

            Vehicles = p.Vehicles
            .Where(v => v.IsActive==true && v.Status != SvVehicleStatus.Maintenance)   // filtering here
            .Select(v => new SvMapVehicleDto
            {
                VehicleId = v.Id,
                Name = v.Name,
                RegistrationNo = v.RegistrationNo,
                VehicleType = v.VehicleType.ToString(),
                MapIconKey = v.MapIconKey,
                LastLatitude = v.LastLatitude,
                LastLongitude = v.LastLongitude,
                Status = v.Status.ToString()
            })
            .ToList()

          }).ToList();


        return result;
    }
}

