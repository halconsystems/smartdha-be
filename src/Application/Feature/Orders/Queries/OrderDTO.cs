using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;

public class OrderDTO
{
    public string? OrderUniqueId { get; set; }
    public OrderType OrderType { get; set; }
    public Guid Id { get; set; }
    public Guid ServiceId {  get; set; }
    public LaundryService? LaundryService { get; set; }
    public string? ServiceName { get; set; }
    public string? PackageName { get; set; }
    public string? TotalPrice { get; set; }
    public string? ItemCount { get; set; }
    public Guid PackageId { get; set; }
    public LaundryPackaging? LaundryPackaging { get; set; }

    public List<Guid>? OrderSummariesId { get; set; }
    public List<OrderSummary>? OrderSummaries { get; set; }
    public List<Guid>? OrderDTiD { get; set; }
    public List<PaymentDTSetting>? PaymentDTSettings { get; set; }
    public Guid OrderConfirmId { get; set; }
    public ConfirmedOrder? ConfirmedOrder { get; set; }

    public Guid DeliveryDetailId {  get; set; }
    public DeliveryDetails? DeliveryDetails { get; set; }
    public DateTime? OrderDate { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public OrderStatus? OrderStatus {  get; set; }
    public OrderDispatchStatus? OrderDispatchStatus {  get; set; }
    public string? ShopName { get; set; }

}
