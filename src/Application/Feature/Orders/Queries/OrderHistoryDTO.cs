using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Queries;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;

public class OrderHistoryDTO
{
    public string? OrderUniqueId { get; set; }
    public OrderType OrderType { get; set; }
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public LaundryService? LaundryService { get; set; }
    public string? ServiceName { get; set; }
    public string? PackageName { get; set; }
    public string? TotalPrice { get; set; }
    public string? ItemCount { get; set; }
    public Guid PackageId { get; set; }
    public Guid PickupAssignedByUserId { get; set; }
    public Guid DeliverAssignedByUserId { get; set; }
    public ApplicationUser? PickUpByUser { get; set; }
    public Guid PickupDriverId { get; set; }
    public ApplicationUser? PickupDrivers { get; set; }
    public Guid DeliverDriverId { get; set; }
    public ApplicationUser? DeliverDrivers { get; set; }
    public ApplicationUser? DeliverByUser { get; set; }
    public LaundryPackaging? LaundryPackaging { get; set; }

    public List<Guid>? OrderSummariesId { get; set; }
    public List<OrderSummary>? OrderSummaries { get; set; }
    public List<Guid>? OrderDTiD { get; set; }
    public List<PaymentDTSetting>? PaymentDTSettings { get; set; }
    public Guid OrderConfirmId { get; set; }
    public Domain.Entities.LMS.OrderDispatch? OrderDispatches { get; set; }


    public Guid DeliveryDetailId { get; set; }
    public DeliveryDetails? DeliveryDetails { get; set; }
    public DateTime? OrderDate { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public string? ShopName { get; set; }

    public List<LaundryCategoryDTO>? LaundryCategories { get; set; }
    public List<LaundryItemsDTO>? LaundryItems { get; set; }
    public List<OrderDTSettingDTO>? OrderDTs { get; set; }
    public Domain.Entities.LMS.Shops? Shops { get; set; }


    //public DateTime? AcceptPickupAt { get; set; }
    //public DateTime? RiderArrivedToPickupAddressAt { get; set; }
    //public DateTime? ParcelPickedParcelFromAddressAt { get; set; }
    //public DateTime? RiderArrivedOnShopAt { get; set; }

    //public DateTime? AcceptDeliveredAt { get; set; }
    //public DateTime? RiderArrivedToPickDeliveryFromShopAt { get; set; }
    //public DateTime? ParcelPickedParcelFromShopAt { get; set; }
    //public DateTime? PickUpAssignedAt { get; set; }
    //public DateTime? DeliveredAssignedAt { get; set; }
    //public string? OrderRemarks { get; set; }

}
