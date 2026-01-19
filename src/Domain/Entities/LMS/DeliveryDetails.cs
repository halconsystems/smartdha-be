using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MediatR.NotificationPublishers;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class DeliveryDetails : BaseAuditableEntity
{
    public Guid OrderId { get; set; }
    public Orders? Orders { get; set; }
    [Required]
    public string City { get; set; } = default!;
    [Required]
    public string CompleteAddress { get; set; } = default!;
    public string? NearByLandMark { get; set; }
    [Required]
    public float Latitude { get; set; } = default!;
    [Required]
    public float Longitude { get; set; } = default!;
    [Required]
    public string PhoneNumber { get; set; } = default!;
    public string? DeliveryInstruction { get; set; }
    [Required]
    public decimal subTotal { get; set; } = default!;
    [Required]
    public decimal Total { get; set; } = default!;
    public decimal? Discount { get; set; }
    public decimal? Taxes { get; set; }
    public decimal? paidAmount { get; set; }
    public decimal? RemainingBalance { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
