using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.LMS;

namespace DHAFacilitationAPIs.Domain.Entities;

public class OrderPaymentIntents :BaseAuditableEntity
{
    [Required]
    public Guid? OrderId { get; set; }
    public Orders? Orders { get; set; }
    public string? UniqueOrderId { get; set; }
    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal AmountToCollect { get; set; }          // deposit or full

    public bool IsDeposit { get; set; } = true;
    public PaymentIntentStatus Status { get; set; } = PaymentIntentStatus.RequiresPayment;
    public PaymentProvider Provider { get; set; } = PaymentProvider.None;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }              // optional payment window

    [MaxLength(100)] public string? ProviderIntentId { get; set; } // gateway identifier
    public string? Meta { get; set; }                               // JSON payload etc.

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

}
