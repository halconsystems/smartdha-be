using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvMapPoints;
public record GetSvMapPointsQuery() : IRequest<SvMapResponseDto>;
public class GetSvMapPointsQueryHandler: IRequestHandler<GetSvMapPointsQuery, SvMapResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IVehicleLocationStore _vehicleLocationStore;
    private readonly UserManager<ApplicationUser> _userManager;


    public GetSvMapPointsQueryHandler(IApplicationDbContext context, IVehicleLocationStore vehicleLocationStore, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _vehicleLocationStore = vehicleLocationStore;
        _userManager = userManager;
    }

    public async Task<SvMapResponseDto> Handle(GetSvMapPointsQuery request, CancellationToken ct)
    {
        // 1️⃣ Load points only
        var pointDtos = await _context.SvPoints
            .Where(p => p.IsActive==true)
            .Select(p => new SvMapPointDto
            {
                PointId = p.Id,
                Code = p.Code,
                Name = p.Name,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Address = p.Address
            })
            .ToListAsync(ct);

        // 2️⃣ Load all active vehicles
        var vehicles = await _context.SvVehicles
            .Where(v => v.IsActive==true && v.Status != SvVehicleStatus.Maintenance)
            .ToListAsync(ct);

        // 3️⃣ Load active dispatches (vehicle → panic)
        var dispatches = await _context.PanicDispatches
            .Include(d => d.PanicRequest)
                .ThenInclude(p => p.EmergencyType)
            .Where(d =>
                d.Status == PanicDispatchStatus.Assigned ||
                d.Status == PanicDispatchStatus.Accepted || d.Status == PanicDispatchStatus.Arrived)
            .ToListAsync(ct);

        var driverIds = vehicles
                .Where(v => !string.IsNullOrWhiteSpace(v.DriverUserId))
                .Select(v => v.DriverUserId!)
                .Distinct()
                .ToList();

        var drivers = await _userManager.Users
            .AsNoTracking()
            .Where(u => driverIds.Contains(u.Id))
            .ToListAsync(ct);

        var driverLookup = drivers.ToDictionary(d => d.Id, d => d);



        var jsonLocations = new Dictionary<Guid, VehicleLocationDto?>();

        foreach (var v in vehicles)
        {
            jsonLocations[v.Id] = await _vehicleLocationStore.GetLocationAsync(v.Id);
        }


       

        // 4️⃣ Build vehicle list with PanicInfo (flat list)
        var vehicleDtos = vehicles.Select(v =>
        {

            // Try get JSON live location
            var json = jsonLocations[v.Id];

            // Use JSON if exists, else DB
            double? lat = json?.Latitude ?? v.LastLatitude;
            double? lng = json?.Longitude ?? v.LastLongitude;
            DateTime? lastTime = json?.LastLocationUpdateAt ?? v.LastLocationAt;

            var lastDispatch = dispatches
                .Where(d => d.SvVehicleId == v.Id)
                .OrderByDescending(d => d.AssignedAt)
                .FirstOrDefault();

            PanicInfoDto? panicInfo = null;

            if (lastDispatch != null)
            {
                var panic = lastDispatch.PanicRequest;
                panicInfo = new PanicInfoDto
                {
                    PanicId = panic.Id,
                    CaseNo = panic.CaseNo,
                    Status = panic.Status,
                    Latitude = panic.Latitude,
                    Longitude = panic.Longitude,
                    EmergencyName = panic.EmergencyType.Name,
                    AssignedAt = lastDispatch.AssignedAt
                };
            }

            DriverInfoDto? driverInfo = null;

            if (!string.IsNullOrWhiteSpace(v.DriverUserId) &&
                driverLookup.TryGetValue(v.DriverUserId!, out var d))
            {
                driverInfo = new DriverInfoDto
                {
                    DriverId = d.Id,
                    Name = d.Name,
                    MobileNo = d.MobileNo ?? d.RegisteredMobileNo ?? "",
                    CNIC = d.CNIC ?? "",
                    IsActive = d.IsActive
                };
            }


            return new SvMapVehicleDto
            {
                VehicleId = v.Id,
                Name = v.Name,
                RegistrationNo = v.RegistrationNo,
                VehicleType = v.VehicleType.ToString(),
                MapIconKey = v.MapIconKey,
                // 🔥 Override with JSON if exists
                LastLatitude = lat,
                LastLongitude = lng,
                LastLocationAt = lastTime,

                Status = v.Status.ToString(),
                PanicInfo = panicInfo,
                // ✅ ADD THIS
                Driver = driverInfo
            };
        })
        .ToList();

        // 5️⃣ Final output
        return new SvMapResponseDto
        {
            Points = pointDtos,
            Vehicles = vehicleDtos
        };
    }


}

