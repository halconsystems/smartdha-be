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
using Microsoft.AspNetCore.SignalR;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.AssignPanicToVehicle;
public record AssignPanicToVehicleCommand(
    Guid PanicId,
    Guid VehicleId,
    string? ControlRoomRemarks
) : IRequest<Guid>;


public class AssignPanicToVehicleCommandHandler
    : IRequestHandler<AssignPanicToVehicleCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPanicRealtime _realtime;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;
    private readonly IVehicleLocationStore _vehicleLocationStore;

    public AssignPanicToVehicleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPanicRealtime realtime,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService,
        IVehicleLocationStore vehicleLocationStore)
    {
        _context = context;
        _currentUser = currentUser;
        _realtime = realtime;
        _userManager = userManager;
        _notificationService = notificationService;
        _vehicleLocationStore = vehicleLocationStore;
    }

    public async Task<Guid> Handle(AssignPanicToVehicleCommand request, CancellationToken ct)
    {

        var existingDispatch = await _context.PanicDispatches
                .Where(x => x.PanicRequestId == request.PanicId &&
                x.SvVehicleId == request.VehicleId &&
                x.Status != PanicDispatchStatus.Completed && x.Status != PanicDispatchStatus.Cancelled)
                .OrderByDescending(x => x.AssignedAt)
                .FirstOrDefaultAsync(ct);

        if (existingDispatch != null)
        {
            // Return existing dispatch (NO transaction, NO update)
            return existingDispatch.Id;
        }


        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            // Get Panic
            var panic = await _context.PanicRequests
                .Include(x => x.EmergencyType)
                .FirstOrDefaultAsync(p => p.Id == request.PanicId, ct)
                ?? throw new NotFoundException(nameof(PanicRequest), request.PanicId.ToString());

            // Get Vehicle
            var vehicle = await _context.SvVehicles
                .FirstOrDefaultAsync(v => v.Id == request.VehicleId, ct)
                ?? throw new NotFoundException(nameof(SvVehicle), request.VehicleId.ToString());

            if (vehicle.Status != SvVehicleStatus.Available)
                throw new InvalidOperationException("Vehicle is not available for dispatch.");

            // Create Dispatch record
            var dispatch = new PanicDispatch
            {
                PanicRequestId = panic.Id,
                SvVehicleId = vehicle.Id,
                Status = PanicDispatchStatus.Assigned,
                AssignedAt = DateTime.Now,
                AssignedByUserId = _currentUser.UserId.ToString(),
                ControlRoomRemarks = request.ControlRoomRemarks
            };

            _context.PanicDispatches.Add(dispatch);

            // Update Panic Status → Acknowledged
            panic.Status = PanicStatus.Acknowledged;

            // Mark vehicle busy
            vehicle.Status = SvVehicleStatus.Busy;

            var id = panic.RequestedByUserId.ToString();
            var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);


            var json = await _vehicleLocationStore.GetLocationAsync(vehicle.Id);

            // Choose final location (JSON → fallback DB)
            double? finalLat = json?.Latitude ?? vehicle.LastLatitude;
            double? finalLng = json?.Longitude ?? vehicle.LastLongitude;
            DateTime? finalTimestamp = json?.LastLocationUpdateAt ?? vehicle.LastLocationAt;


            var updateDto = new PanicUpdatedRealtimeDto
            {
                // PANIC INFO
                PanicId = panic.Id,
                CaseNo = panic.CaseNo,
                PanicStatus = panic.Status,
                Latitude = panic.Latitude,
                Longitude = panic.Longitude,
                Created = panic.Created,
                Address = panic.Address ?? "",
                Note = panic.Notes ?? "",
                MobileNumber = panic.MobileNumber ?? "",
                EmergencyType = panic.EmergencyType.Name,

                // USER INFO
                RequestedByName = user?.Name
                  ?? "Unknown",

                RequestedByEmail = user?.Email
                   ?? "",

                RequestedByPhone = user?.MobileNo
                   ?? user?.RegisteredMobileNo
                   ?? panic.MobileNumber
                   ?? "",

                RequestedByUserType = user?.UserType
                      ?? UserType.NonMember,

                // DISPATCH INFO
                DispatchId = dispatch.Id,
                AssignedAt = dispatch.AssignedAt,

                // VEHICLE INFO
                VehicleId = vehicle.Id,
                VehicleName = vehicle.Name,
                RegistrationNo = vehicle.RegistrationNo,
                VehicleType = vehicle.VehicleType.ToString(),
                VehicleStatus = vehicle.Status.ToString(),
                MapIconKey = vehicle.MapIconKey,
                //LastLatitude = vehicle.LastLatitude,
                //LastLongitude = vehicle.LastLongitude,
                //LastLocationAt = vehicle.LastLocationAt
                LastLatitude = finalLat,
                LastLongitude = finalLng,
                LastLocationAt = finalTimestamp,
            };

            // Send notification to driver app
            var driverUserId = vehicle.DriverUserId;

            var lastLogin = await _context.UserLoginAudits
            .Where(x => x.UserId == driverUserId && x.IsSuccess)
            .OrderByDescending(x => x.LoginAt)
            .FirstOrDefaultAsync(ct);

            var deviceToken = lastLogin?.DeviceToken;

            // Prepare notification title and body
            var title = $"Panic Case #{panic.CaseNo}";
            var body = $"{panic.EmergencyType.Name}: raised by {user?.Name ?? "Unknown"} at {panic.Address}";

            // Prepare full data payload
            var data = new Dictionary<string, string>
            {
                ["PanicId"] = panic.Id.ToString(),
                ["CaseNo"] = panic.CaseNo,
                ["PanicStatus"] = panic.Status.ToString(),
                ["Latitude"] = panic.Latitude.ToString(),
                ["Longitude"] = panic.Longitude.ToString(),
                ["Created"] = panic.Created.ToString("O"),
                ["Address"] = panic.Address ?? "",
                ["Note"] = panic.Notes ?? "",
                ["MobileNumber"] = panic.MobileNumber ?? "",
                ["EmergencyType"] = panic.EmergencyType.Name,
                ["ControlRoomRemarks"] = request.ControlRoomRemarks ?? "",

                ["RequestedByName"] = user?.Name ?? "Unknown",
                ["RequestedByEmail"] = user?.Email ?? "",
                ["RequestedByPhone"] =
                    user?.MobileNo
                    ?? user?.RegisteredMobileNo
                    ?? panic.MobileNumber
                    ?? "",

                ["RequestedByUserType"] = (user?.UserType ?? UserType.NonMember).ToString(),

                ["DispatchId"] = dispatch.Id.ToString(),
                ["AssignedAt"] = dispatch.AssignedAt.ToString("O"),

                ["VehicleId"] = vehicle.Id.ToString(),
                ["VehicleName"] = vehicle.Name,
                ["RegistrationNo"] = vehicle.RegistrationNo,
                ["VehicleType"] = vehicle.VehicleType.ToString()
            };

            //await _context.SaveChangesAsync(ct);
            //await transaction.CommitAsync(ct);
            // return dispatch.Id;

            //Send push notification to driver
            if (!string.IsNullOrWhiteSpace(deviceToken))
            {
                var notifyResult = await _notificationService.SendFirebaseNotificationAsync(
                   deviceToken,
                   title,
                   body,
                   data,
                   ct
                );

                if (!notifyResult.IsSuccess)
                {
                    await transaction.RollbackAsync(ct);
                    throw new Exception($"Failed to notify driver: {notifyResult.ErrorMessage}");
                }
                else
                {
                    await _context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                    // Send realtime update to ALL dispatchers
                    await _realtime.SendPanicUpdatedAsync(updateDto, ct);
                    return dispatch.Id;
                }
            }

            await transaction.RollbackAsync(ct);
            throw new InvalidOperationException("Failed to notify driver:");
        }
        catch (Exception e) 
        {
            await transaction.RollbackAsync(ct);
            throw new InvalidOperationException(e.ToString());
        }

    }
}


