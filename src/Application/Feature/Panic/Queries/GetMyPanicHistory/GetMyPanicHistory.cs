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

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyPanicHistory;
public record GetMyPanicHistoryQuery() : IRequest<List<PanicUpdatedRealtimeDto>>;

public class GetMyPanicHistoryQueryHandler
    : IRequestHandler<GetMyPanicHistoryQuery, List<PanicUpdatedRealtimeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetMyPanicHistoryQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUser = currentUser;
        _userManager = userManager;
    }

    public async Task<List<PanicUpdatedRealtimeDto>> Handle(GetMyPanicHistoryQuery request, CancellationToken ct)
    {
        var driverId = _currentUser.UserId.ToString();

        if (string.IsNullOrWhiteSpace(driverId))
            throw new UnauthorizedAccessException("Driver not authenticated.");

        // 1️⃣ Get the vehicle assigned to this driver
        var vehicle = await _context.SvVehicles
            .FirstOrDefaultAsync(v => v.DriverUserId == driverId, ct);

        if (vehicle == null)
            throw new InvalidOperationException("No vehicle assigned to driver.");

        // 2️⃣ Get all dispatches for this vehicle
        var dispatches = await _context.PanicDispatches
            .Where(d => d.SvVehicleId == vehicle.Id)
            .Include(d => d.PanicRequest)
                .ThenInclude(p => p.EmergencyType)
            .OrderByDescending(d => d.AssignedAt)
            .ToListAsync(ct);

        var result = new List<PanicUpdatedRealtimeDto>();

        foreach (var dispatch in dispatches)
        {
            var panic = dispatch.PanicRequest;

            var requester = await _userManager.FindByIdAsync(panic.RequestedByUserId.ToString());

            result.Add(new PanicUpdatedRealtimeDto
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
                ArrivedAt = dispatch.ArrivedAt,
                CompletedAt = dispatch.CompletedAt,

                VehicleId = vehicle.Id,
                VehicleName = vehicle.Name,
                RegistrationNo = vehicle.RegistrationNo,
                VehicleType = vehicle.VehicleType.ToString(),
                VehicleStatus = vehicle.Status.ToString(),
                MapIconKey = vehicle.MapIconKey,
                LastLatitude = vehicle.LastLatitude,
                LastLongitude = vehicle.LastLongitude,
                LastLocationAt = vehicle.LastLocationAt
            });
        }

        return result;
    }
}

