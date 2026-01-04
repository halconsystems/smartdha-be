using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetCurrentUserProfile;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.DriverProfile;
public record GetUserProfileQuery : IRequest<DriverProfileDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, DriverProfileDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;
    private readonly IVehicleLocationStore _vehicleLocationStore;
    private readonly ICurrentUserService _currentUser;

    public GetUserProfileQueryHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context,
        IVehicleLocationStore vehicleLocationStore,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _context = context;
        _vehicleLocationStore = vehicleLocationStore;
        _currentUser = currentUser;
    }

    public async Task<DriverProfileDto> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new NotFoundException("User not found");

        // Get vehicle assigned to this user
        var vehicle = await _context.SvVehicles
            .FirstOrDefaultAsync(v => v.DriverUserId == user.Id, ct);

        // VEHICLE + JSON LOCATION
        double? lat = null;
        double? lng = null;
        DateTime? ts = null;

        if (vehicle != null)
        {
            var json = await _vehicleLocationStore.GetLocationAsync(vehicle.Id);

            lat = json?.Latitude ?? vehicle.LastLatitude;
            lng = json?.Longitude ?? vehicle.LastLongitude;
            ts = json?.LastLocationUpdateAt ?? vehicle.LastLocationAt;
        }

        return new DriverProfileDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email ?? "",
            Phone = user.MobileNo ?? user.RegisteredMobileNo ?? "",
            CNIC = user.CNIC ?? "",
            UserType = user.UserType,

            HasAssignedVehicle = vehicle != null,
            VehicleId = vehicle?.Id,
            VehicleName = vehicle?.Name,
            RegistrationNo = vehicle?.RegistrationNo,
            VehicleType = vehicle?.VehicleType.ToString(),
            VehicleStatus = vehicle?.Status.ToString(),
            MapIconKey = vehicle?.MapIconKey,

            LastLatitude = lat,
            LastLongitude = lng,
            LastLocationAt = ts
        };
    }
}

