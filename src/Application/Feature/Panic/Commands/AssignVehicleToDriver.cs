using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands;
public class AssignVehicleToDriverCommand : IRequest<string>
{
   public Guid VehicleId { get; set; }
}
public class AssignVehicleToDriverCommandHandler : IRequestHandler<AssignVehicleToDriverCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public AssignVehicleToDriverCommandHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<string> Handle(AssignVehicleToDriverCommand req, CancellationToken ct)
    {
        // Get logged-in user (Driver)
        var userId = _currentUserService.UserId.ToString();

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("Invalid token: User not found.");

        var driver = await _userManager.FindByIdAsync(userId);
        if (driver == null)
            throw new UnauthorizedAccessException("Driver does not exist.");

        var vehicle = await _context.SvVehicles
            .FirstOrDefaultAsync(x => x.Id == req.VehicleId, ct);

        if (vehicle == null)
            throw new NotFoundException("Vehicle not found.");

        // Step 1: Close previous assignment if the vehicle was assigned before
        if (vehicle.DriverUserId != null)
        {
            var previousHistory = await _context.SvVehicleAssignmentHistories
                .Where(x => x.VehicleId == vehicle.Id && x.UnassignedAt == null)
                .FirstOrDefaultAsync(ct);

            if (previousHistory != null)
                previousHistory.UnassignedAt = DateTime.UtcNow;
        }

        // Step 2: Assign the vehicle to this driver
        vehicle.DriverUserId = userId;

        // Step 3: New assignment history entry
        var newHistory = new SvVehicleAssignmentHistory
        {
            VehicleId = vehicle.Id,
            DriverUserId = userId,
            AssignedAt = DateTime.UtcNow
        };

        _context.SvVehicleAssignmentHistories.Add(newHistory);

        // Step 4: Save
        await _context.SaveChangesAsync(ct);

        return "Vehicle assigned successfully";
    }
}

