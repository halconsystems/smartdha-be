using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.ShopVehicles.Command;

public record AssignedDriverToVehiclesCommand(
    Guid VehicleId
) : IRequest<Guid>;


public class AssignedDriverToVehiclesCommandHandler
    : IRequestHandler<AssignedDriverToVehiclesCommand, Guid>
{
   
    private readonly ILaundrySystemDbContext _laundrySystemDb;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;


    public AssignedDriverToVehiclesCommandHandler(
        ICurrentUserService currentUser,
        ILaundrySystemDbContext laundrySystemDb,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        
        _currentUser = currentUser;
        _laundrySystemDb = laundrySystemDb;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<Guid> Handle(AssignedDriverToVehiclesCommand request, CancellationToken ct)
    {

        var userId = _currentUserService.UserId.ToString();
        Guid UsedId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("Invalid token: User not found.");

        var driver = await _userManager.FindByIdAsync(userId);
        if (driver == null)
            throw new UnauthorizedAccessException("Driver does not exist.");

        var SHopDriver = await _laundrySystemDb.ShopDrivers
           .FirstOrDefaultAsync(x => x.DriverId == UsedId);

        if (SHopDriver == null)
            throw new NotFoundException("User is not assigned to any Shop.");

        var vehicle = await _laundrySystemDb.ShopVehicles
            .FirstOrDefaultAsync(x => x.Id == request.VehicleId, ct);

        if (vehicle == null)
            throw new NotFoundException("Vehicle not found.");

        // Step 1: Close previous assignment if the vehicle was assigned before
        if (vehicle.DriverUserId != null)
        {
            var previousHistory = await _laundrySystemDb.ShopVehicleAssignmentHistories
                .Where(x => x.VehicleId == vehicle.Id && x.UnassignedAt == null)
                .FirstOrDefaultAsync(ct);

            if (previousHistory != null)
                previousHistory.UnassignedAt = DateTime.UtcNow;
        }

        vehicle.DriverUserId = UsedId;
        vehicle.Status = ShopVehicleStatus.Available;

        // Step 3: New assignment history entry
        var newHistory = new ShopVehicleAssignmentHistory
        {
            VehicleId = vehicle.Id,
            DriverUserId = userId,
            AssignedAt = DateTime.UtcNow
        };

        _laundrySystemDb.ShopVehicleAssignmentHistories.Add(newHistory);

        

        //var shopVehicles = await _laundrySystemDb.ShopVehicles
        //    .Where(x => x.Id == request.VehicleId &&
        //    x.Status == ShopVehicleStatus.Available)
        //    .FirstOrDefaultAsync();

        //if(shopVehicles == null)
        //    throw new InvalidOperationException("Vehicle is not available for Assignment.");




        //shopVehicles.DriverUserId = UsedId;
        //shopVehicles.Status = ShopVehicleStatus.Available;
        await _laundrySystemDb.SaveChangesAsync(ct);

        return vehicle.Id;
    }
}



