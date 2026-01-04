using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicles;
public record GetSvVehiclesQuery() : IRequest<List<SvVehicleListDto>>;

public class GetSvVehiclesQueryHandler
    : IRequestHandler<GetSvVehiclesQuery, List<SvVehicleListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetSvVehiclesQueryHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<SvVehicleListDto>> Handle(GetSvVehiclesQuery request, CancellationToken ct)
    {
        var vehicles = await _context.SvVehicles
            .Include(v => v.SvPoint)
            .Where(v => v.IsActive == true)
            .ToListAsync(ct);

        var result = new List<SvVehicleListDto>();

        foreach (var v in vehicles)
        {
            // -------------------------------
            // 1. Check if vehicle is assigned
            // -------------------------------
            bool isAssigned = v.DriverUserId != null;
            ApplicationUser? driver = null;
            DateTime? assignedAt = null;

            if (isAssigned)
            {
                // Pull assigned driver
                driver = await _userManager.FindByIdAsync(v.DriverUserId!);

                // Get last assignment history
                var lastHistory = await _context.SvVehicleAssignmentHistories
                    .Where(x => x.VehicleId == v.Id)
                    .OrderByDescending(x => x.AssignedAt)
                    .FirstOrDefaultAsync(ct);

                assignedAt = lastHistory?.AssignedAt;
            }

            result.Add(new SvVehicleListDto
            {
                VehicleId = v.Id,
                Name = v.Name,
                RegistrationNo = v.RegistrationNo,
                VehicleType = v.VehicleType.ToString(),
                Status = v.Status.ToString(),
                MapIconKey = v.MapIconKey,

                LastLatitude = v.LastLatitude,
                LastLongitude = v.LastLongitude,
                LastLocationAt = v.LastLocationAt,

                PointId = v.SvPointId,
                PointName = v.SvPoint?.Name ?? "",

                DriverUserId = v.DriverUserId,
                IsActive = v.IsActive,

                // NEW FIELDS:
                IsDriverAssigned = isAssigned,
                AssignedDriverName = driver?.Name,
                AssignedDriverPhone = driver?.MobileNo,
                AssignedDriverCnic = driver?.CNIC,   // (change if CNIC stored somewhere else)
                AssignedAt = assignedAt
            });
        }

        return result;
    }
}

