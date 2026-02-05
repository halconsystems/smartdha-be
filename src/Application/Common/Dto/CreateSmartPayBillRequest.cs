using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Dto;
public class CreateSmartPayBillRequest
{
    public string ConsumerNumber { get; set; } = default!;   // e.g. 60385452999
    public string ConsumerDetail { get; set; } = default!;
    public string ReferenceInfo { get; set; } = default!;
    public string Institution { get; set; } = default!;

    public string BillAmount { get; set; } = default!;
    public string LateFee { get; set; } = "0";
    public string AmountAfterDueDate { get; set; } = default!;

    public string DueDate { get; set; } = default!;          // dd/MM/yyyy
    public string ExpiryDate { get; set; } = default!;       // dd/MM/yyyy
    public string BillGenerateOn { get; set; } = default!;   // dd/MM/yyyy

    public string PaymentStatus { get; set; } = default!;

    public string UserId { get; set; } = default!;
}

