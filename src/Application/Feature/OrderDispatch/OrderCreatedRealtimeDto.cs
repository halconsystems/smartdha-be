using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.OrderDispatch;


public sealed record OrderCreatedRealtimeDto(
    Guid Id,
    string OrderNo,
    OrderType OrderType,
    string EmergencyName,
    string PickUpAddress,
    string DeliveryAddress,
    OrderStatus Status,
    DateTime CreatedUtc,
    string Address,
    string Note,
    string MobileNumber,

    // 👇 User details
    string RequestedByName,
    string RequestedByEmail,
    string RequestedByPhone,
    UserType RequestedByUserType
);
