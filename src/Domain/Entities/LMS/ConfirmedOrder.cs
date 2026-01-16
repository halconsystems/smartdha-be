using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class ConfirmedOrder : BaseAuditableEntity
{
    public Guid OrderId     { get; set; }
    public Orders? Orders { get; set; }
    [Required]
    public string PickUpAddress { get; set; } = default!;
    [Required]
    public string DeliverAddress { get; set; } = default!;
    public Guid ShopId { get; set; }
    public Shops? Shops { get; set; }
    public OrderStatus OrderStatus { get; set; }
    [MaxLength(36)] public string? AssignedToUserId { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? RiderOnTheWayAt { get; set; }
    public DateTime? RiderArrivedToAddressAt { get; set; }
    public DateTime? ParcelPickedParcelFromAddressAt { get; set; }
    public DateTime? RiderArrivedOnShopAt { get; set; }
    public DateTime? WashnPressProcessAt { get; set; }
    public DateTime? ParcelReadyAt { get; set; }
    public DateTime? DeliveredToRiderAt { get; set; }
    public DateTime? RiderWayFromShopToHomeAt { get; set; }
    public DateTime? RiderArrivedToDeliveredAddressAt { get; set; }
    [Required]
    public decimal AmountToCollect { get; set; }  // total amount for COD
    public decimal? CollectedAmount { get; set; } // amount collected by rider
    public bool IsPaid => CollectedAmount >= AmountToCollect;
    public string? BasketId { get; set; } 

}
