using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.LMS;

namespace DHAFacilitationAPIs.Domain.Entities;

public class OrderPayment :BaseAuditableEntity
{
    [Required]
    public Guid orderPaymentIntentId { get; set; }
    public OrderPaymentIntents OrderPaymentIntents { get; set; } = default!;
    [Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Initiated;
    public DateTime? PaidAt { get; set; }
    public PaymentProvider Provider { get; set; } = PaymentProvider.None;
    [MaxLength(100)] public string? ProviderTransactionId { get; set; }
    public string? RawResponse { get; set; } // gateway payload for audits
    public Guid? OrderId {  get; set; }
    public Orders? Orders { get; set; }
}
