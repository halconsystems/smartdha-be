using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;

public enum OrderStatus
{
    Confirmed = 1, //Used
    Acknowledged = 2, //Used
    InProgress = 3,
    Resolved = 4,
    Cancelled = 5,
    ArrivedToPickup = 6,
    DeliveredToShop = 7,
    PickUpParcel = 8,


}
public enum OrderDispatchStatus
{
    Confirmed = 1,
    Acknowledged = 2,
    AssignedToRider = 3,
    RiderOnTheWay = 4,
    RiderArrivedToAddress = 5,
    ParcelPickedParcelFromAddress = 6,
    RiderWayToTheShop = 7,
    RiderArrivedOnShop = 8,
    DeliveredToShop = 9,
    WashnPressProcess = 10,
    ParcelReady = 11,
    DeliveredToRider = 12,
    RiderWayFromShopToHome = 13,
    RiderArrivedToDeliveredAddress = 14,
    ParcelDelivered = 15,
    Completed = 16,
    Cancelled = 17,
    RiderAcceptPickUp = 18,
    RiderArrivedToPickDeliveryFromShop = 19,


}
