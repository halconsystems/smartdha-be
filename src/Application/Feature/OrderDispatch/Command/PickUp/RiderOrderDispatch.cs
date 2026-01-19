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

namespace DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.PickUp;

public class RiderPickupOrderDispatch : IRequest<string>
{
    public bool PickUp { get; set; } = false;
    public bool Accept { get; set; } = false;
    public bool ReachedToPickup { get; set; } = false;
    public bool ReachedToDelivered { get; set; } = false;
    public bool PickUpParcel { get; set; } = false;
    public bool Delivered { get; set; } = false;
    public Guid DispatchId { get; set; }
}

public class ReachedPickupOrderDispatchHandler
    : IRequestHandler<RiderPickupOrderDispatch, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ILaundrySystemDbContext _laundry;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IOrderRealTime _realtime;
    private readonly IGeocodingService _geocodingService;
    private readonly IVehicleLocationStore _fileService;

    public ReachedPickupOrderDispatchHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUser,
        IOrderRealTime realtime,
        IGeocodingService geocodingService,
        IVehicleLocationStore fileService, ILaundrySystemDbContext laundry)
    {
        _context = context;
        _userManager = userManager;
        _currentUser = currentUser;
        _realtime = realtime;
        _geocodingService = geocodingService;
        _fileService = fileService;
        _laundry = laundry;
    }

    public async Task<string> Handle(RiderPickupOrderDispatch request, CancellationToken ct)
    {
        Domain.Entities.LMS.OrderDispatch? dispatch = null;
        // Get logged-in driver
        var userId = _currentUser.UserId.ToString();
        if (userId == null)
            throw new UnauthorizedAccessException("Driver not authenticated.");

        var driver = await _userManager.FindByIdAsync(userId);
        if (driver == null)
            throw new UnauthorizedAccessException("Driver not found.");

        // Fetch dispatch
        if(request.PickUp == true)
        {
            dispatch = await _laundry.OrderDispatches
            .Include(d => d.Orders)
            .Include(d => d.PickupShopVehicles)
            .FirstOrDefaultAsync(d => d.Id == request.DispatchId, ct)
            ?? throw new NotFoundException("Dispatch not found");
        }
        else
        {
            dispatch = await _laundry.OrderDispatches
            .Include(d => d.Orders)
            .Include(d => d.DeliverShopVehicles)
            .FirstOrDefaultAsync(d => d.Id == request.DispatchId, ct)
            ?? throw new NotFoundException("Dispatch not found");
        }
        if (dispatch == null)
        {
            throw new NotFoundException("Dispatch not found");
        }

        var getOrder = await _laundry.Orders
            .FirstOrDefaultAsync(p => p.Id == dispatch.OrderId, ct);
        if (getOrder != null)
        {
            //For Pickup
            if(request.PickUp == true)
            {
                if (request.Accept)
                {
                    getOrder.AcceptPickupAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;
                }
                else if (request.ReachedToPickup)
                {
                    getOrder.RiderArrivedToPickupAddressAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;
                }
                else if (request.PickUpParcel)
                {
                    getOrder.ParcelPickedParcelFromAddressAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;
                }
                else
                {
                    getOrder.RiderArrivedOnShopAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;
                }


                // Security check → only assigned driver can accept
                if (dispatch.PickupDriverId.ToString() != userId)
                    throw new UnauthorizedAccessException("You are not assigned to this vehicle.");

                // Validate state
                if (dispatch.Status != OrderDispatchStatus.AssignedToRider)
                    throw new InvalidOperationException("Dispatch cannot be accepted at this stage.");

                var location = await _fileService.GetLocationAsync(dispatch.PickupVehicleId);

                if (location != null)
                {
                    dispatch.AcceptedAtLatitude = location.Latitude;
                    dispatch.AcceptedAtLongitude = location.Longitude;
                    dispatch.LastLocationUpdateAt = location.LastLocationUpdateAt;
                    dispatch.AcceptedAtAddress = await _geocodingService.GetAddressFromLatLngAsync(location.Latitude, location.Longitude, ct);

                    // 📏 Calculate distance (panic → vehicle)
                    dispatch.DistanceFromOrderKm =
                        GeoDistanceHelper.CalculateKm(
                            dispatch.Orders.PickupLatitude,
                            dispatch.Orders.PickupLongitude,
                            location.Latitude,
                            location.Longitude
                        );
                }
                if (request.Accept)
                {
                    // Update dispatch status
                    dispatch.Status = OrderDispatchStatus.RiderOnTheWay;

                    // Optional: update vehicle status (if required)
                    dispatch.PickupShopVehicles.Status = ShopVehicleStatus.Busy;
                }
                if (request.ReachedToPickup)
                {
                    dispatch.Status = OrderDispatchStatus.RiderArrivedToAddress;

                    // Optional: update vehicle status (if required)
                    dispatch.PickupShopVehicles.Status = ShopVehicleStatus.Busy;
                }
                else if (request.PickUpParcel)
                {
                    dispatch.Status = OrderDispatchStatus.ParcelPickedParcelFromAddress;

                    // Optional: update vehicle status (if required)
                    dispatch.PickupShopVehicles.Status = ShopVehicleStatus.Busy;
                }
                else
                {
                    dispatch.Status = OrderDispatchStatus.DeliveredToShop;

                    // Optional: update vehicle status (if required)
                    dispatch.PickupShopVehicles.Status = ShopVehicleStatus.Available;
                }
            }
            else
            {
                if (request.Accept)
                {
                    getOrder.AcceptDeliveredAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;
                }
                else if (request.ReachedToDelivered)
                {
                    getOrder.RiderArrivedToPickDeliveryFromShopAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;
                }
                else if (request.PickUpParcel)
                {
                    getOrder.ParcelPickedParcelFromShopAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;
                }
                else
                {
                    getOrder.ParcelDeliveredParcelAddressAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.Resolved;
                }

                // Security check → only assigned driver can accept
                if (dispatch.DeliverDriverId.ToString() != userId)
                    throw new UnauthorizedAccessException("You are not assigned to this vehicle.");

                // Validate state
                if (dispatch.Status != OrderDispatchStatus.AssignedToRider)
                    throw new InvalidOperationException("Dispatch cannot be accepted at this stage.");

                var location = await _fileService.GetLocationAsync(dispatch.DeliverVehicleId);

                if (location != null)
                {
                    dispatch.AcceptedAtLatitude = location.Latitude;
                    dispatch.AcceptedAtLongitude = location.Longitude;
                    dispatch.LastLocationUpdateAt = location.LastLocationUpdateAt;
                    dispatch.AcceptedAtAddress = await _geocodingService.GetAddressFromLatLngAsync(location.Latitude, location.Longitude, ct);

                    // 📏 Calculate distance (panic → vehicle)
                    dispatch.DistanceFromOrderKm =
                        GeoDistanceHelper.CalculateKm(
                            dispatch.Orders.PickupLatitude,
                            dispatch.Orders.PickupLongitude,
                            location.Latitude,
                            location.Longitude
                        );
                }
                if (request.Accept)
                {
                    // Update dispatch status
                    dispatch.Status = OrderDispatchStatus.RiderOnTheWay;

                    // Optional: update vehicle status (if required)
                    dispatch.DeliverShopVehicles.Status = ShopVehicleStatus.Busy;
                }
                if (request.ReachedToDelivered)
                {
                    dispatch.Status = OrderDispatchStatus.RiderArrivedToPickDeliveryFromShop;

                    // Optional: update vehicle status (if required)
                    dispatch.PickupShopVehicles.Status = ShopVehicleStatus.Busy;
                }
                else if (request.PickUpParcel)
                {
                    dispatch.Status = OrderDispatchStatus.RiderWayFromShopToHome;

                    // Optional: update vehicle status (if required)
                    dispatch.PickupShopVehicles.Status = ShopVehicleStatus.Busy;
                }
                else
                {
                    dispatch.Status = OrderDispatchStatus.ParcelDelivered;

                    // Optional: update vehicle status (if required)
                    dispatch.PickupShopVehicles.Status = ShopVehicleStatus.Available;
                }
            }
        }
        

        await _context.SaveChangesAsync(ct);

        // Prepare realtime DTO
        var updateDto = new OrderUpdateRealTimeDTO
        {
            OrderId = dispatch.OrderId,
            DispatchId = dispatch.Id,
            PanicStatus = dispatch.Orders.OrderStatus,
            AssignedAt = dispatch.PickUpAssignedAt == DateTime.MinValue ? dispatch.DeliveredAssignedAt : dispatch.PickUpAssignedAt,
            AcceptedAt = dispatch.Orders.AcceptPickupAt == DateTime.MinValue ? dispatch.Orders.AcceptDeliveredAt : dispatch.Orders.AcceptPickupAt,

            // Vehicle Info
            VehicleId = dispatch.PickupVehicleId.ToString() == null ? dispatch.DeliverVehicleId : dispatch.PickupVehicleId,
            VehicleName = dispatch.PickupShopVehicles == null ? dispatch.DeliverShopVehicles.Name : dispatch.PickupShopVehicles.Name,
            RegistrationNo = dispatch.PickupShopVehicles == null ? dispatch.DeliverShopVehicles.RegistrationNo : dispatch.PickupShopVehicles.RegistrationNo,
            VehicleType = dispatch.PickupShopVehicles == null ?  dispatch.DeliverShopVehicles.VehicleType.ToString() : dispatch.PickupShopVehicles.VehicleType.ToString(),
            VehicleStatus = dispatch.PickupShopVehicles == null ? dispatch.DeliverShopVehicles.Status.ToString() : dispatch.PickupShopVehicles.Status.ToString(),
            MapIconKey = dispatch.PickupShopVehicles == null ? dispatch.DeliverShopVehicles.MapIconKey : dispatch.PickupShopVehicles.MapIconKey,
            LastLatitude = dispatch.PickupShopVehicles  == null ? dispatch.DeliverShopVehicles.LastLatitude : dispatch.PickupShopVehicles.LastLatitude,
            LastLongitude = dispatch.PickupShopVehicles == null ? dispatch.DeliverShopVehicles.LastLongitude : dispatch.PickupShopVehicles.LastLongitude,
            LastLocationAt = dispatch.PickupShopVehicles == null ? dispatch.DeliverShopVehicles.LastLocationAt : dispatch.PickupShopVehicles.LastLocationAt,

            // Driver Info
            RequestedByName = driver.Name,
            RequestedByEmail = driver.Email ?? "",
            RequestedByPhone = driver.PhoneNumber ?? ""
        };

        // Notify control room & dashboards
        await _realtime.SendOrderUpdatedAsync(updateDto, ct);



        return "Assignment successful. Vehicle en route.";
    }

    public static class GeoDistanceHelper
    {
        private const double EarthRadiusKm = 6371;

        public static double CalculateKm(double lat1, double lon1, double lat2, double lon2)
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


