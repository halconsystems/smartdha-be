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

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyAssignedPanic;
public record GetMyAssignedPanicQuery() : IRequest<PanicUpdatedRealtimeDto>;

public class GetMyAssignedPanicQueryHandler
    : IRequestHandler<GetMyAssignedPanicQuery, PanicUpdatedRealtimeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetMyAssignedPanicQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUser = currentUser;
        _userManager = userManager;
    }

    public async Task<PanicUpdatedRealtimeDto> Handle(GetMyAssignedPanicQuery request, CancellationToken ct)
    {
        var driverId = _currentUser.UserId.ToString();

        if (string.IsNullOrWhiteSpace(driverId))
            throw new UnauthorizedAccessException("Driver not logged in.");

        // 1️⃣ Find vehicle assigned to this driver
        var vehicle = await _context.SvVehicles
            .FirstOrDefaultAsync(v => v.DriverUserId == driverId, ct);

        if (vehicle == null)
            throw new InvalidOperationException("No vehicle assigned to this driver.");

        // 2️⃣ Find active panic dispatch for this vehicle
        var dispatch = await _context.PanicDispatches
            .Include(d => d.PanicRequest)
                .ThenInclude(p => p.EmergencyType)
            .Where(d =>
                d.SvVehicleId == vehicle.Id &&
                d.Status != PanicDispatchStatus.Completed &&
                d.Status != PanicDispatchStatus.Cancelled)
            .OrderByDescending(d => d.AssignedAt)
            .FirstOrDefaultAsync(ct);

        if (dispatch == null)
            throw new InvalidOperationException("No active panic assignment found.");

        var panic = dispatch.PanicRequest;

        // 3️⃣ Get requested-by user info
        var requester = await _userManager.FindByIdAsync(panic.RequestedByUserId.ToString());

        // 4️⃣ Build DTO
        return new PanicUpdatedRealtimeDto
        {
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

            RequestedByName = requester?.Name ?? "Unknown",
            RequestedByEmail = requester?.Email ?? "",
            RequestedByPhone =
                requester?.MobileNo
                ?? requester?.RegisteredMobileNo
                ?? panic.MobileNumber
                ?? "",
            RequestedByUserType = requester?.UserType ?? UserType.NonMember,

            DispatchId = dispatch.Id,
            AssignedAt = dispatch.AssignedAt,
            AcceptedAt = dispatch.AcceptedAt,

            VehicleId = vehicle.Id,
            VehicleName = vehicle.Name,
            RegistrationNo = vehicle.RegistrationNo,
            VehicleType = vehicle.VehicleType.ToString(),
            VehicleStatus = vehicle.Status.ToString(),
            MapIconKey = vehicle.MapIconKey,
            LastLatitude = vehicle.LastLatitude,
            LastLongitude = vehicle.LastLongitude,
            LastLocationAt = vehicle.LastLocationAt
        };
    }
}

