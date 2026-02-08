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

namespace DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.PickUp;

public record BulkOrderDispatch
(
    Guid OrderId,
    Guid VehicleId,
    string? OrderRemarks,
    bool? Pickup
);

public class DisptachDTO
{
    public Guid? Id { get; set; }
    public string? RequesterName { get; set; }
    public string? OrderNo { get; set; }
    public string? ShopName { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? tax { get; set; }
    public decimal? discount { get; set; }
    public decimal? subtotal { get; set; }
    public PaymentMethod? Payment { get; set; }
    public decimal? AmountToCollect { get; set; }
    public decimal? CollectedAmount { get; set; }
    public DateOnly? CreatedDate { get; set; }
}

public record AssignBulkOrderDispatchCommand(
   List<BulkOrderDispatch> bulkOrders
) : IRequest<SuccessResponse<List<DisptachDTO>>>;


public class AssignBulkOrderDispatchCommandHandler
    : IRequestHandler<AssignBulkOrderDispatchCommand, SuccessResponse<List<DisptachDTO>>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILaundrySystemDbContext _laundrySystemDb;

    public AssignBulkOrderDispatchCommandHandler(
        ICurrentUserService currentUser,
        ILaundrySystemDbContext laundrySystemDb,
        UserManager<ApplicationUser> userManager)
    {
        _currentUser = currentUser;
        _laundrySystemDb = laundrySystemDb;
        _userManager = userManager;
    }

    public async Task<SuccessResponse<List<DisptachDTO>>> Handle(AssignBulkOrderDispatchCommand request, CancellationToken ct)
    {

        List<Domain.Entities.LMS.OrderDispatch>? existingDispatch = null;
        if (request.bulkOrders.Any(x => x.Pickup == true))
        {
            existingDispatch = await _laundrySystemDb.OrderDispatches
               .Where(x => request.bulkOrders.Select(r => r.OrderId).Contains(x.OrdersId) &&
               request.bulkOrders.Select(r => r.VehicleId).Contains(x.PickupVehicleId ?? Guid.Empty) &&
               x.Status != OrderDispatchStatus.Completed && x.Status != OrderDispatchStatus.Cancelled)
               .OrderByDescending(x => x.PickUpAssignedAt)
               .ToListAsync(ct);
        }
        else
        {
            existingDispatch = await _laundrySystemDb.OrderDispatches
               .Where(x => request.bulkOrders.Select(x => x.OrderId).Contains(x.OrdersId) &&
               request.bulkOrders.Select(r => r.VehicleId).Contains(x.DeliverVehicleId ?? Guid.Empty) &&
               x.Status != OrderDispatchStatus.Completed && x.Status != OrderDispatchStatus.Cancelled)
               .OrderByDescending(x => x.DeliveredAssignedAt)
               .ToListAsync(ct);
        }

        if (existingDispatch.Any())
        {
            // Return existing dispatch (NO transaction, NO update)
            return Success.Empty<List<DisptachDTO>>("Order Already Dispatched");
        }

        try
        {
            // Get Panic
            var order = await _laundrySystemDb.Orders
                .Where(p => request.bulkOrders.Select(x => x.OrderId).Contains(p.Id))
                .ToListAsync(ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.LMS.Orders),"Order Not Found");

            var userDetails = await _userManager.Users
                .Where(x => order.Select(x => x.UserId).Contains(x.Id))
                .AsNoTracking()
                .ToListAsync(ct);

            var shopDetails = await _laundrySystemDb.Shops
                .Where(x => order.Select(o => o.ShopId).Contains(x.Id))
                .FirstOrDefaultAsync(ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.LMS.Shops), "Delivery Details Not Found");

            var DeliveryDetails = await _laundrySystemDb.DeliveryDetails
                .Where(x => order.Select(o => o.Id).Contains(x.OrderId))
                .ToListAsync(ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.LMS.DeliveryDetails), "Delivery Details Not Found");

            // Get Vehicle
            var vehicle = await _laundrySystemDb.ShopVehicles
                .FirstOrDefaultAsync(v => request.bulkOrders.Select(r => r.VehicleId).Contains(v.Id), ct)
                ?? throw new NotFoundException(nameof(SvVehicle), "Shop Vehicle Not Found");

            if (vehicle.Status != ShopVehicleStatus.Available)
                throw new InvalidOperationException("Vehicle is not available for dispatch.");

            List<Domain.Entities.LMS.OrderDispatch>? dispatch = null;
            if (request.bulkOrders.Any(x => x.Pickup == true))
            {
                dispatch = request.bulkOrders.Select(x => new Domain.Entities.LMS.OrderDispatch
                {
                    OrdersId = x.OrderId,
                    PickupVehicleId = vehicle.Id,
                    Status = OrderDispatchStatus.AssignedToRider,
                    PickUpAssignedAt = DateTime.Now,
                    PickupAssignedByUserId = _currentUser.UserId,
                    OrderRemarks = x.OrderRemarks,
                    PickupDriverId = vehicle.DriverUserId,

                }).ToList();

                _laundrySystemDb.OrderDispatches.AddRange(dispatch);

                // Update Panic Status → Acknowledged
                await _laundrySystemDb.Orders
                    .Where(d => request.bulkOrders.Select(x => x.OrderId).Contains(d.Id))
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(d => d.OrderStatus, OrderStatus.Acknowledged)
                    , ct);

                List<DisptachDTO> disptachDTOs = order.Select(x => new DisptachDTO {
                    Id = dispatch.FirstOrDefault(d => d.OrdersId == x.Id)?.Id,
                    RequesterName = userDetails?.FirstOrDefault(u => u.Id == x.UserId)?.Name,
                    OrderNo = x.UniqueFormID,
                    ShopName = shopDetails.DisplayName,
                    Address = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.CompleteAddress,
                    TotalPrice = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.Total,
                    tax = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.Taxes,
                    discount = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.Discount,
                    subtotal = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.subTotal,
                    Payment = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.PaymentMethod,
                    AmountToCollect = x.AmountToCollect,
                    CollectedAmount = x.CollectedAmount,
                    CreatedDate = DateOnly.FromDateTime(x.Created)
                }).ToList();

                //order.OrderStatus = OrderStatus.Acknowledged;

                await _laundrySystemDb.SaveChangesAsync(ct);

                return Success.Created(
                    disptachDTOs,
                    "Order Dispatch Sucessfully"
                    );

            }
            else
            {
                var dispatchIds = await _laundrySystemDb.OrderDispatches
                    .Where(d =>
                        request.bulkOrders.Select(x => x.OrderId).Contains(d.OrdersId) &&
                        d.Status != OrderDispatchStatus.Completed &&
                        d.Status != OrderDispatchStatus.Cancelled)
                    .Select(d => new
                    {
                        d.Id,
                        d.OrdersId
                    })
                    .ToListAsync(ct);


                // Fetch dispatch
                await _laundrySystemDb.OrderDispatches
                    .Where(d =>
                        request.bulkOrders.Select(x => x.OrderId).Contains(d.OrdersId) &&
                        d.Status != OrderDispatchStatus.Completed &&
                        d.Status != OrderDispatchStatus.Cancelled)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(d => d.DeliverDriverId, vehicle.DriverUserId)
                        .SetProperty(d => d.DeliverAssignedByUserId, _currentUser.UserId)
                        .SetProperty(d => d.DeliveredAssignedAt, DateTime.Now)
                        .SetProperty(d => d.DeliverVehicleId, vehicle.Id)
                        .SetProperty(d => d.Status, OrderDispatchStatus.DeliveredToRider),
                        ct);

                List<DisptachDTO> disptachDTOs = order.Select(x => new DisptachDTO
                {
                    Id = dispatchIds.FirstOrDefault(d => d.OrdersId == x.Id)?.Id,
                    RequesterName = userDetails?.FirstOrDefault(u => u.Id == x.UserId)?.Name,
                    OrderNo = x.UniqueFormID,
                    ShopName = shopDetails.DisplayName,
                    Address = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.CompleteAddress,
                    TotalPrice = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.Total,
                    tax = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.Taxes,
                    discount = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.Discount,
                    subtotal = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.subTotal,
                    Payment = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.PaymentMethod,
                    AmountToCollect = x.AmountToCollect,
                    CollectedAmount = x.CollectedAmount,
                    CreatedDate = DateOnly.FromDateTime(x.Created)
                }).ToList();

                await _laundrySystemDb.SaveChangesAsync(ct);

                return Success.Created(
                   disptachDTOs,
                   "Order Dispatch Sucessfully"
                   );

            }

        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.ToString());
        }

    }
}




