using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.Payment;

namespace DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetMyBillDetail;
public class MyBillDetailDto
{
    public Guid PaymentBillId { get; set; }
    public string Title { get; set; } = default!;
    public string SourceSystem { get; set; } = default!;
    public string? EntityName { get; set; }
    public decimal BillAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public DateTime BillGeneratedOn { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public PaymentBillStatus PaymentStatus { get; set; }
    public MyBillTransactionDto Transactions { get; set; } = new();
}
public class MyBillTransactionDto
{
    public decimal Amount { get; set; }
    public string Status { get; set; } = default!;
    public string? GatewayTransactionId { get; set; }
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string PaymentName { get; set; } = default!;
    public string IssuerName { get; set; } = default!;
}
