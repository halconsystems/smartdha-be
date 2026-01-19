using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class Orders :BaseAuditableEntity
{
    [Required]
    public string UniqueFormID { get; set; } = default!;
    public Guid ServiceId { get; set; }
    public LaundryService? LaundryService { get; set; }
    public Guid PackageId { get; set; }
    public LaundryPackaging? LaundryPackaging { get; set; }
    [Required]
    public string UserId { get; set; } = default!;
    public OrderType OrderType { get; set; }

    [Required]
    public string PickUpAddress { get; set; } = default!;
    public double PickupLatitude { get; set; }
    public double PickupLongitude { get; set; }

    [Required]
    public string DeliverAddress { get; set; } = default!;
    public double DropLatitude { get; set; }
    public double DropLongitude { get; set; }

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Confirmed;

    public DateTime? AcceptPickupAt { get; set; }
    public DateTime? AcceptDeliveredAt { get; set; }
    public DateTime? RiderArrivedToPickupAddressAt { get; set; }
    public DateTime? ParcelPickedParcelFromAddressAt { get; set; }
    public DateTime? RiderArrivedOnShopAt { get; set; }
    public DateTime? RiderArrivedToPickDeliveryFromShopAt { get; set; }
    public DateTime? ParcelPickedParcelFromShopAt { get; set; }
    public DateTime? ParcelDeliveredParcelAddressAt { get; set; }
    
    public DateTime? WashnPressProcessAt { get; set; }
    public DateTime? ParcelReadyAt { get; set; }
    public DateTime? DeliveredToRiderAt { get; set; }
    [Required]
    public decimal AmountToCollect { get; set; }  // total amount for COD
    public decimal? CollectedAmount { get; set; } // amount collected by rider
    public bool IsPaid => CollectedAmount >= AmountToCollect;

    public string? BasketId { get; set; }
    public Guid ShopId { get; set; }
    public Shops? Shops { get; set; }

}
