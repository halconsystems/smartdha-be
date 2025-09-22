using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_Payment : BaseAuditableEntity
{
    public Guid RequestId { get; set; }
    public OLH_BowserRequest Request { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "PKR";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string Provider { get; set; } = "COD";   // Easypaisa/JazzCash/Stripe/COD
    public string? ProviderPaymentId { get; set; }
    public string? ProviderIntentId { get; set; }
    public string? AuthorizationCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AuthorizedAt { get; set; }
    public DateTime? CapturedAt { get; set; }
    public DateTime? VoidedAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? MetaJson { get; set; }           // masked payloads
}
