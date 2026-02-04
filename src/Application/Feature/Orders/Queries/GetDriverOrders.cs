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

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;

public record GetMyAssignedOrderQuery(bool PickUp) : IRequest<List<DriverOrderDTO>>;

public class GetMyAssignedOrderQueryHandler
    : IRequestHandler<GetMyAssignedOrderQuery, List<DriverOrderDTO>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetMyAssignedOrderQueryHandler(
        ILaundrySystemDbContext context,
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUser = currentUser;
        _userManager = userManager;
    }

    public async Task<List<DriverOrderDTO>> Handle(GetMyAssignedOrderQuery request, CancellationToken ct)
    {
        var driverId = _currentUser.UserId.ToString();
        var driverUserId = _currentUser.UserId;

        if (string.IsNullOrWhiteSpace(driverId))
            throw new UnauthorizedAccessException("Driver not logged in.");

        // 1️⃣ Find vehicle assigned to this driver
        var vehicle = await _context.ShopVehicles
            .FirstOrDefaultAsync(v => v.DriverUserId == driverUserId, ct);

        if (vehicle == null)
            throw new InvalidOperationException("No vehicle assigned to this driver.");

        List<Domain.Entities.LMS.OrderDispatch>? orderDispatch = new List<Domain.Entities.LMS.OrderDispatch>();
        if (request.PickUp)
        {
            // 2️⃣ Find active panic dispatch for this vehicle
            orderDispatch = await _context.OrderDispatches
            .Include(d => d.Orders)
            .Where(d =>
                d.PickupVehicleId == vehicle.Id &&
                d.Status != OrderDispatchStatus.Completed &&
                d.Status != OrderDispatchStatus.Cancelled)
            .OrderByDescending(d => d.PickUpAssignedAt)
            .ToListAsync(ct);
        }
        else
        {
            orderDispatch = await _context.OrderDispatches
           .Include(d => d.Orders)
           .Where(d =>
               d.PickupVehicleId == vehicle.Id &&
               d.Status != OrderDispatchStatus.Completed &&
               d.Status != OrderDispatchStatus.Cancelled)
           .OrderByDescending(d => d.PickUpAssignedAt)
           .ToListAsync(ct);
        }



        if (orderDispatch == null)
            throw new InvalidOperationException("No active Orders assignment found.");

        var order = orderDispatch.Select(x => x.Orders);

        var deliveryDetails = await _context.DeliveryDetails
            .Where(x => order.Select(x => x.Id).Contains(x.OrderId))
            .FirstOrDefaultAsync(ct);

        // 3️⃣ Get requested-by user info


        var requester = await _userManager.Users
            .Where(o => (order.Select(x => x.UserId).Contains(o.Id))).ToListAsync(ct);

        if(requester == null)
            throw new InvalidOperationException("No Requester Order found.");

        var requesterDict = requester.ToDictionary(u => u.Id);
        var dispatchDict = orderDispatch.ToDictionary(d => d.OrdersId);

        var resuls = order.Select(x =>
        {
            requesterDict.TryGetValue(x.UserId, out var req);
            dispatchDict.TryGetValue(x.Id, out var dispatch);

            return new DriverOrderDTO
            {
                OrderId = x.Id,
                OrderNo = x.UniqueFormID,
                PanicStatus = x.OrderStatus,
                Created = x.Created,
                PickUpAddress = x.PickUpAddress ?? "",

                RequestedByName = req?.Name ?? string.Empty,
                RequestedByEmail = req?.Email ?? string.Empty,
                RequestedByPhone = deliveryDetails?.PhoneNumber ?? "",
                RequestedByUserType = req?.UserType ?? UserType.NonMember,

                DispatchId = dispatch?.Id,
                AssignedAt = dispatch?.PickUpAssignedAt,

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
        }).ToList();

        return resuls;

        

    }
}


