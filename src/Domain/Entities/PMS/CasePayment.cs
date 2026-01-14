using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CasePayment : BaseAuditableEntity
{
    public Guid CaseId { get; set; }
    public PropertyCase Case { get; set; } = default!;

    public Guid? VoucherId { get; set; }
    public CaseVoucher? Voucher { get; set; }

    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Initiated;

    public decimal Amount { get; set; }

    // Gateway transaction refs for Card/Online
    [MaxLength(100)]
    public string? TransactionId { get; set; }

    [MaxLength(200)]
    public string? GatewayResponse { get; set; }

    // For cash collection
    [MaxLength(150)]
    public string? CollectedBy { get; set; }

    public DateTime? PaidAt { get; set; }
}

