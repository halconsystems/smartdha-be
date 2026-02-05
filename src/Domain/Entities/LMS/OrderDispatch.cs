using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class OrderDispatch :BaseAuditableEntity
{
    public Guid OrdersId {  get; set; }
    public Orders Orders { get; set; } = default!;
    public Guid? DeliverVehicleId {  get; set; }
    public ShopVehicles? DeliverShopVehicles { get; set; }

    public Guid? PickupVehicleId { get; set; }  
    public ShopVehicles? PickupShopVehicles { get; set; }
    public OrderDispatchStatus Status { get; set; } = OrderDispatchStatus.Confirmed;

    public DateTime PickUpAssignedAt { get; set; }
    public DateTime DeliveredAssignedAt { get; set; }
    public Guid? PickupAssignedByUserId { get; set; } = default!; 
    public Guid? PickupDriverId { get; set; } = default!;
    public Guid? DeliverAssignedByUserId { get; set; } = default!;
    public Guid? DeliverDriverId { get; set; } = default!;
    public bool IsPickup {  get; set; }
    public bool IsDelivered {  get; set; }
    public string? OrderRemarks { get; set; }

    public double? AcceptedAtLatitude { get; set; }
    public double? AcceptedAtLongitude { get; set; }
    [MaxLength(300)] public string? AcceptedAtAddress { get; set; }
    public double? DistanceFromOrderKm { get; set; }
}
