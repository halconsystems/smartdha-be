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
using static Dapper.SqlMapper;

namespace DHAFacilitationAPIs.Application.Feature.ShopVehicles.Command;

public record AssignedDriverToVehiclesCommand(
    Guid VehicleId
) : IRequest<Guid>;


public class AssignedDriverToVehiclesCommandHandler
    : IRequestHandler<AssignedDriverToVehiclesCommand, Guid>
{
   
    private readonly ILaundrySystemDbContext _laundrySystemDb;
    private readonly ICurrentUserService _currentUser;

    public AssignedDriverToVehiclesCommandHandler(
        ICurrentUserService currentUser,
        ILaundrySystemDbContext laundrySystemDb)
    {
        
        _currentUser = currentUser;
        _laundrySystemDb = laundrySystemDb;
    }

    public async Task<Guid> Handle(AssignedDriverToVehiclesCommand request, CancellationToken ct)
    {
        Guid UsedId = _currentUser.UserId;

        var SHopDriver = await _laundrySystemDb.ShopDrivers
            .FirstOrDefaultAsync(x => x.DriverId == UsedId);

        var shopVehicles = await _laundrySystemDb.ShopVehicles
            .Where(x => x.Id == request.VehicleId &&
            x.Status == ShopVehicleStatus.Available)
            .FirstOrDefaultAsync();

        if(shopVehicles == null)
            throw new InvalidOperationException("Vehicle is not available for Assignment.");


        shopVehicles.DriverUserId = UsedId;
        shopVehicles.Status = ShopVehicleStatus.Available;
        await _laundrySystemDb.SaveChangesAsync(ct);

        return shopVehicles.Id;
    }
}



