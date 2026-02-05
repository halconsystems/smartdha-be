using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.Payment;

namespace DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetMyPaidBills;
public class MyPaidBillSummaryDto
{
    public Guid PaymentBillId { get; set; }
    public string SourceVoucherNo { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string SourceSystem { get; set; } = default!; // PMS / CLUB / SCHOOL / SMARTPAY
    public PaymentBillStatus PaymentStatus { get; set; }
    public decimal BillAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime? Paymentdate { get; set; }

}

