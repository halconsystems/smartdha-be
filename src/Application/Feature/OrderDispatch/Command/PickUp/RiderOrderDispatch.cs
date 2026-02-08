using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
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
        var UserID = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException("Driver not authenticated.");

        var driver = await _userManager.FindByIdAsync(userId);
        if (driver == null)
            throw new UnauthorizedAccessException("Driver not found.");

        //var vehicleDetails = _laundry.ShopVehicles.Where(x => x.DriverUserId == UserID).FirstOrDefault();
        //if (vehicleDetails == null)
        //    throw new UnauthorizedAccessException("Vehicle not found.");


        // Fetch dispatch
        if (request.PickUp == true)
        {
            dispatch = await _laundry.OrderDispatches
            .Include(d => d.Orders)
            .Include(d => d.PickupShopVehicles)
            .FirstOrDefaultAsync(d => d.Id == request.DispatchId && d.Status == OrderDispatchStatus.AssignedToRider, ct)
            ?? throw new NotFoundException("Dispatch not found");
        }
        else
        {
            dispatch = await _laundry.OrderDispatches
            .Include(d => d.Orders!)
            .Include(d => d.DeliverShopVehicles)
            .FirstOrDefaultAsync(d => d.Id == request.DispatchId && d.Status == OrderDispatchStatus.ParcelReady, ct)
            ?? throw new NotFoundException("Dispatch not found");
        }
        if (dispatch == null)
        {
            throw new NotFoundException("Dispatch not found");
        }

        var getOrder = await _laundry.Orders
            .FirstOrDefaultAsync(p => p.Id == dispatch.OrdersId, ct);
        if (getOrder != null)
        {
            //For Pickup
            if(request.PickUp == true)
            {
                if (request.Accept)
                {
                    getOrder.AcceptPickupAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;

                    dispatch.Status = OrderDispatchStatus.RiderOnTheWay;
                }
                else if (request.ReachedToPickup)
                {
                    getOrder.RiderArrivedToPickupAddressAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;

                    dispatch.Status = OrderDispatchStatus.RiderArrivedToAddress;
                }
                else if (request.PickUpParcel)
                {
                    getOrder.ParcelPickedParcelFromAddressAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;

                    dispatch.Status = OrderDispatchStatus.ParcelPickedParcelFromAddress;
                }
                else
                {
                    getOrder.RiderArrivedOnShopAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;

                    dispatch.Status = OrderDispatchStatus.DeliveredToShop;
                }

            }
            else
            {
                if (request.Accept)
                {
                    getOrder.AcceptDeliveredAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;

                    dispatch.Status = OrderDispatchStatus.RiderOnTheWay;
                }
                else if (request.ReachedToDelivered)
                {
                    getOrder.RiderArrivedToPickDeliveryFromShopAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;

                    dispatch.Status = OrderDispatchStatus.RiderArrivedToPickDeliveryFromShop;
                }
                else if (request.PickUpParcel)
                {
                    getOrder.ParcelPickedParcelFromShopAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.InProgress;

                    dispatch.Status = OrderDispatchStatus.RiderWayFromShopToHome;
                }
                else if(request.Delivered)
                {
                    getOrder.ParcelDeliveredParcelAddressAt = DateTime.Now;
                    getOrder.OrderStatus = OrderStatus.Resolved;

                    dispatch.Status = OrderDispatchStatus.ParcelDelivered;
                }
            }
        }
        

        await _laundry.SaveChangesAsync(ct);




        return dispatch.Id.ToString();
    }

    //public static class GeoDistanceHelper
    //{
    //    private const double EarthRadiusKm = 6371;

    //    public static double CalculateKm(double lat1, double lon1, double lat2, double lon2)
    //    {
    //        double dLat = ToRad(lat2 - lat1);
    //        double dLon = ToRad(lon2 - lon1);

    //        lat1 = ToRad(lat1);
    //        lat2 = ToRad(lat2);

    //        double a =
    //            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
    //            Math.Cos(lat1) * Math.Cos(lat2) *
    //            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

    //        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    //        return EarthRadiusKm * c;
    //    }

    //    private static double ToRad(double deg) => deg * (Math.PI / 180);
}


