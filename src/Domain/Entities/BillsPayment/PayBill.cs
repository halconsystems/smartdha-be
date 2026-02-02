using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.Payment;

namespace DHAFacilitationAPIs.Domain.Entities.BillsPayment;
public class PayBill : BaseAuditableEntity
{
    // User who owns the bill (PMS / Club / School / DHA Security)
    public string UserId { get; set; } = default!;

    // Entity display name (Property, Club, School, Directorate)
    public string? EntityName { get; set; }

    // Source module (PMS / Club / School / DHA Security)
    public string SourceSystem { get; set; } = default!;

    // Original voucher ID from source system
    public Guid SourceVoucherId { get; set; }

    // Original voucher number from source system
    public string SourceVoucherNo { get; set; } = default!;

    // Unique payment bill identifier
    public Guid PaymentBillId { get; set; }

    // Bill title for display (Case Fee, Club Booking, School Fee)
    public string Title { get; set; } = default!;

    // Total bill amount
    public decimal BillAmount { get; set; }

    // Total amount paid so far
    public decimal PaidAmount { get; set; }

    // Remaining payable amount
    public decimal OutstandingAmount { get; set; }

    // Payment due date (if applicable)
    public DateTime? DueDate { get; set; }

    // Bill expiry date (auto-expire unpaid bills)
    public DateTime? ExpiryDate { get; set; }

    // Current bill status (Generated / Paid / Expired)
    public PaymentBillStatus PaymentStatus { get; set; }

    // Last successful payment date
    public DateTime? LastPaymentDate { get; set; }

    // Last payment gateway reference number
    public string? LastAuthNo { get; set; }

    // Settlement merchant code (Club / School / Directorate)
    public string? MerchantCode { get; set; }

    // Bill creation timestamp
    public DateTime BillGeneratedOn { get; set; }

    // Payment attempts for this bill

    public decimal? LateFee { get; set; } = 0;
    public decimal? AmountAfterDueDate { get; set; } = 0;

    public ICollection<PayTransaction> Transactions { get; set; } = new List<PayTransaction>();
}

