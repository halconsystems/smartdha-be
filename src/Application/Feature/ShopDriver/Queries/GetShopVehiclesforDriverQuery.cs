using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicles;
using DHAFacilitationAPIs.Application.Feature.ShopVehicles.Queries;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.ShopDriver.Queries;


public record GetShopVehiclesforDriverQuery() : IRequest<List<ShopVehicleDTO>>;

public class GetShopVehiclesforDriverQueryHandler
    : IRequestHandler<GetShopVehiclesforDriverQuery, List<ShopVehicleDTO>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetShopVehiclesforDriverQueryHandler(
        ILaundrySystemDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<ShopVehicleDTO>> Handle(GetShopVehiclesforDriverQuery request, CancellationToken ct)
    {
        var vehicles = await _context.ShopVehicles
            .Where(v => v.IsActive == true && v.IsDeleted == false && v.DriverUserId == null)
            .ToListAsync(ct);

        var result = new List<ShopVehicleDTO>();

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
                driver = await _userManager.FindByIdAsync(v.DriverUserId.ToString()!);

                // Get last assignment history
                var lastHistory = await _context.ShopVehicleAssignmentHistories
                    .Where(x => x.VehicleId == v.Id)
                    .OrderByDescending(x => x.AssignedAt)
                    .FirstOrDefaultAsync(ct);

                assignedAt = lastHistory?.AssignedAt;
            }

            result.Add(new ShopVehicleDTO
            {
                VehicleId = v.Id,
                Name = v.Name,
                RegistrationNo = v.RegistrationNo,
                VehicleTypeString = v.VehicleType.ToString(),
                Statustring = v.Status.ToString(),
                MapIconKey = v.MapIconKey,

                LastLatitude = v.LastLatitude,
                LastLongitude = v.LastLongitude,
                LastLocationAt = v.LastLocationAt,

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

