using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.Payment;

namespace DHAFacilitationAPIs.Domain.Entities.BillsPayment;
public class PayBill : BaseAuditableEntity
{
    // PMS / CLUB / SCHOOL
    public string SourceSystem { get; set; } = default!;
    public Guid SourceVoucherId { get; set; }
    public string SourceVoucherNo { get; set; } = default!;
    public string Title { get; set; } = default!;
    // CLUB / PROPERTY / SCHOOL
    public string EntityType { get; set; } = default!;
    public Guid? EntityId { get; set; }
    public string? EntityName { get; set; }
    // Identity UserId
    public string UserId { get; set; } = default!;
    // Frozen at bill creation
    public string MerchantCode { get; set; } = default!;
    public string Currency { get; set; } = "PKR";
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public PaymentBillStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<PayTransaction> Transactions { get; set; } = new List<PayTransaction>();
}

