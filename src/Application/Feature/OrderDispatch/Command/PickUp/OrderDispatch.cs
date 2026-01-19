using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;


namespace DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.PickUp;

public record AssignOrderDispatchCommand(
    Guid OrderId,
    Guid VehicleId,
    string? OrderRemarks,
    bool? Pickup
) : IRequest<Guid>;


public class AssignOrderDispatchCommandHandler
    : IRequestHandler<AssignOrderDispatchCommand, Guid>
{
    private readonly ICurrentUserService _currentUser;
    private readonly ILaundrySystemDbContext _laundrySystemDb;

    public AssignOrderDispatchCommandHandler(
        ICurrentUserService currentUser,
        ILaundrySystemDbContext laundrySystemDb)
    {
        _currentUser = currentUser;
        _laundrySystemDb = laundrySystemDb;
    }

    public async Task<Guid> Handle(AssignOrderDispatchCommand request, CancellationToken ct)
    {

        Domain.Entities.LMS.OrderDispatch? existingDispatch = null;
        if (request.Pickup == true)
        {
             existingDispatch = await _laundrySystemDb.OrderDispatches
                .Where(x => x.OrdersId == request.OrderId &&
                x.PickupVehicleId == request.VehicleId &&
                x.Status != OrderDispatchStatus.Completed && x.Status != OrderDispatchStatus.Cancelled)
                .OrderByDescending(x => x.PickUpAssignedAt)
                .FirstOrDefaultAsync(ct);
        }
        else
        {
             existingDispatch = await _laundrySystemDb.OrderDispatches
                .Where(x => x.OrdersId == request.OrderId &&
                x.DeliverVehicleId == request.VehicleId &&
                x.Status != OrderDispatchStatus.Completed && x.Status != OrderDispatchStatus.Cancelled)
                .OrderByDescending(x => x.DeliveredAssignedAt)
                .FirstOrDefaultAsync(ct);
        }

        if (existingDispatch != null)
        {
            // Return existing dispatch (NO transaction, NO update)
            return existingDispatch.Id;
        }

        try
        {
            // Get Panic
            var order = await _laundrySystemDb.Orders
                .FirstOrDefaultAsync(p => p.Id == request.OrderId, ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.LMS.Orders), request.OrderId.ToString());

            var DeliveryDetails = await _laundrySystemDb.DeliveryDetails
                .FirstOrDefaultAsync(x => x.OrderId == order.Id, ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.LMS.DeliveryDetails), request.OrderId.ToString());

            // Get Vehicle
            var vehicle = await _laundrySystemDb.ShopVehicles
                .FirstOrDefaultAsync(v => v.Id == request.VehicleId, ct)
                ?? throw new NotFoundException(nameof(SvVehicle), request.VehicleId.ToString());

            if (vehicle.Status != ShopVehicleStatus.Available)
                throw new InvalidOperationException("Vehicle is not available for dispatch.");

            Domain.Entities.LMS.OrderDispatch? dispatch = null;
            if (request.Pickup == true)
            {
                // Create Dispatch record
                dispatch = new Domain.Entities.LMS.OrderDispatch
                {
                    OrdersId = order.Id,
                    PickupVehicleId = vehicle.Id,
                    Status = OrderDispatchStatus.AssignedToRider,
                    PickUpAssignedAt = DateTime.Now,
                    PickupAssignedByUserId = _currentUser.UserId,
                    OrderRemarks = request.OrderRemarks,
                    PickupDriverId = vehicle.DriverUserId,
                };

                _laundrySystemDb.OrderDispatches.Add(dispatch);

                // Update Panic Status → Acknowledged
                order.OrderStatus = OrderStatus.Acknowledged;
            }
            else
            {
                // Fetch dispatch
                dispatch = await _laundrySystemDb.OrderDispatches
                    .Include(d => d.Orders)
                    .FirstOrDefaultAsync(d => d.OrdersId == request.OrderId, ct)
                    ?? throw new NotFoundException("Dispatch not found", request.OrderId.ToString());

                order.DeliveredToRiderAt = DateTime.Now;
                order.OrderStatus = OrderStatus.InProgress;

                dispatch.DeliverDriverId = vehicle.DriverUserId;
                dispatch.DeliverAssignedByUserId = _currentUser.UserId;
                dispatch.DeliveredAssignedAt = DateTime.Now;
                dispatch.DeliverVehicleId = request.VehicleId;
                dispatch.Status = OrderDispatchStatus.DeliveredToRider;
            }

            // Mark vehicle busy
            vehicle.Status = ShopVehicleStatus.Busy;

            await _laundrySystemDb.SaveChangesAsync(ct);
            return dispatch.Id;

        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.ToString());
        }

    }
}




