using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.AcceptPanicDispatch;
public class AcceptPanicDispatchCommand : IRequest<string>
{
    public Guid DispatchId { get; set; }
}

public class AcceptPanicDispatchCommandHandler
    : IRequestHandler<AcceptPanicDispatchCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IPanicRealtime _realtime;
    private readonly IGeocodingService _geocodingService;
    private readonly IVehicleLocationStore _fileService;

    public AcceptPanicDispatchCommandHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUser,
        IPanicRealtime realtime,
        IGeocodingService geocodingService,
        IVehicleLocationStore fileService)
    {
        _context = context;
        _userManager = userManager;
        _currentUser = currentUser;
        _realtime = realtime;
        _geocodingService = geocodingService;
        _fileService = fileService;
    }

    public async Task<string> Handle(AcceptPanicDispatchCommand request, CancellationToken ct)
    {
        // Get logged-in driver
        var userId = _currentUser.UserId.ToString();
        if (userId == null)
            throw new UnauthorizedAccessException("Driver not authenticated.");

        var driver = await _userManager.FindByIdAsync(userId);
        if (driver == null)
            throw new UnauthorizedAccessException("Driver not found.");

        // Fetch dispatch
        var dispatch = await _context.PanicDispatches
            .Include(d => d.PanicRequest)
            .Include(d => d.SvVehicle)
            .FirstOrDefaultAsync(d => d.Id == request.DispatchId, ct)
            ?? throw new NotFoundException("Dispatch not found");

        var getPanic = await _context.PanicRequests
            .FirstOrDefaultAsync(p => p.Id == dispatch.PanicRequestId, ct);
            if (getPanic != null)
            {
                getPanic.AcknowledgedAt=DateTime.Now;
                getPanic.Status=PanicStatus.Acknowledged;
        }

            // Security check → only assigned driver can accept
            if (dispatch.SvVehicle.DriverUserId != userId)
            throw new UnauthorizedAccessException("You are not assigned to this vehicle.");

        // Validate state
        if (dispatch.Status != PanicDispatchStatus.Assigned)
            throw new InvalidOperationException("Dispatch cannot be accepted at this stage.");

        var location = await _fileService.GetLocationAsync(dispatch.SvVehicleId);

        if (location != null)
        {
            dispatch.AcceptedAtLatitude = location.Latitude;
            dispatch.AcceptedAtLongitude = location.Longitude;
            dispatch.LastLocationUpdateAt = location.LastLocationUpdateAt;
            dispatch.AcceptedAtAddress= await _geocodingService.GetAddressFromLatLngAsync(location.Latitude, location.Longitude, ct);

            // 📏 Calculate distance (panic → vehicle)
            dispatch.DistanceFromPanicKm =
                GeoDistanceHelper.CalculateKm(
                    dispatch.PanicRequest.Latitude,
                    dispatch.PanicRequest.Longitude,
                    location.Latitude,
                    location.Longitude
                );
        }


        // Update dispatch status
        dispatch.Status = PanicDispatchStatus.Accepted;
        dispatch.AcceptedAt = DateTime.Now;


        // Update panic request status
        dispatch.PanicRequest.Status = PanicStatus.InProgress;

        // Optional: update vehicle status (if required)
        dispatch.SvVehicle.Status = SvVehicleStatus.Busy;


        await _context.SaveChangesAsync(ct);

        // Prepare realtime DTO
        var updateDto = new PanicUpdatedRealtimeDto
        {
            PanicId = dispatch.PanicRequestId,
            DispatchId = dispatch.Id,
            PanicStatus = dispatch.PanicRequest.Status,
            AssignedAt = dispatch.AssignedAt,
            AcceptedAt = dispatch.AcceptedAt,

            // Vehicle Info
            VehicleId = dispatch.SvVehicleId,
            VehicleName = dispatch.SvVehicle.Name,
            RegistrationNo = dispatch.SvVehicle.RegistrationNo,
            VehicleType = dispatch.SvVehicle.VehicleType.ToString(),
            VehicleStatus = dispatch.SvVehicle.Status.ToString(),
            MapIconKey = dispatch.SvVehicle.MapIconKey,
            LastLatitude = dispatch.SvVehicle.LastLatitude,
            LastLongitude = dispatch.SvVehicle.LastLongitude,
            LastLocationAt = dispatch.SvVehicle.LastLocationAt,

            // Driver Info
            RequestedByName = driver.Name,
            RequestedByEmail = driver.Email ?? "",
            RequestedByPhone = driver.PhoneNumber ?? ""
        };

        // Notify control room & dashboards
        await _realtime.SendPanicUpdatedAsync(updateDto, ct);



        return "Assignment successful. Vehicle en route.";
    }

    public static class GeoDistanceHelper
    {
        private const double EarthRadiusKm = 6371;

        public static double CalculateKm(double lat1, double lon1,double lat2, double lon2)
        {
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);

            lat1 = ToRad(lat1);
            lat2 = ToRad(lat2);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private static double ToRad(double deg) => deg * (Math.PI / 180);
    }


}
