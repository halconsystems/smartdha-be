using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicles;
public record GetSvVehiclesQuery() : IRequest<List<SvVehicleListDto>>;
public class GetSvVehiclesQueryHandler
    : IRequestHandler<GetSvVehiclesQuery, List<SvVehicleListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSvVehiclesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SvVehicleListDto>> Handle(GetSvVehiclesQuery request, CancellationToken ct)
    {
        var vehicles = await _context.SvVehicles
            .Include(v => v.SvPoint)
            .Where(v => v.IsActive==true)    // return active only
            .ToListAsync(ct);

        return vehicles.Select(v => new SvVehicleListDto
        {
            VehicleId = v.Id,
            Name = v.Name,
            RegistrationNo = v.RegistrationNo,
            VehicleType = v.VehicleType.ToString(),
            Status = v.Status.ToString(),
            MapIconKey = v.MapIconKey,

            LastLatitude = v.LastLatitude,
            LastLongitude = v.LastLongitude,
            LastLocationAtUtc = v.LastLocationAtUtc,

            PointId = v.SvPointId,
            PointName = v.SvPoint.Name,

            DriverUserId = v.DriverUserId,
            IsActive= v.IsActive
        })
        .ToList();
    }
}

