using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.ViewModels;

public class OrderUpdateRealTimeDTO
{
    // PANIC INFO
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = default!;
    public OrderStatus PanicStatus { get; set; }
    public DateTime Created { get; set; }
    public string PickUpAddress { get; set; } = default!;
    public string DeliveryAddress { get; set; } = default!;
    public string Note { get; set; } = default!;
    public string MobileNumber { get; set; } = default!;
    public OrderType OrderType { get; set; } = default!;
    public int OrderCode { get; set; } = default!;
    public string OrderName { get; set; } = default!;

    // USER INFO
    public string RequestedByName { get; set; } = default!;
    public string RequestedByEmail { get; set; } = default!;
    public string RequestedByPhone { get; set; } = default!;
    public UserType RequestedByUserType { get; set; }

    // DISPATCH INFO
    public Guid DispatchId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? OnRouteAt { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // VEHICLE INFO
    public Guid VehicleId { get; set; }
    public string VehicleName { get; set; } = default!;
    public string RegistrationNo { get; set; } = default!;
    public string VehicleType { get; set; } = default!;
    public string VehicleStatus { get; set; } = default!;
    public string? MapIconKey { get; set; }
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public DateTime? LastLocationAt { get; set; }

    public Guid ShopId { get; set; }
    public string ShopName { get; set; } = default!;
    public string OwnerName { get; set; } = default!;
    public string ShopPhoneNumber { get; set; } = default!;
}


