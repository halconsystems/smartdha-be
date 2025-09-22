using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_Refund : BaseAuditableEntity
{
    public Guid PaymentId { get; set; }
    public OLH_Payment Payment { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime RefundedAt { get; set; }
    public string? Reason { get; set; }
    public string? ProviderRefundId { get; set; }
}

