using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.Payment;

namespace DHAFacilitationAPIs.Domain.Entities.BillsPayment;
public class PayTransaction : BaseAuditableEntity
{
    public Guid PayBillId { get; set; }
    public PayBill PayBill { get; set; } = default!;
    public string Gateway { get; set; } = "PAYFAST";
    public string MerchantCode { get; set; } = default!;
    public string MerchantIdUsed { get; set; } = default!;
    // Unique per transaction
    public string BasketId { get; set; } = default!;
    public decimal AttemptAmount { get; set; }
    public string? AccessToken { get; set; }
    public string? RedirectUrl { get; set; }
    public string? GatewayTransactionId { get; set; }
    public PaymentTransactionStatus Status { get; set; }
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
}

