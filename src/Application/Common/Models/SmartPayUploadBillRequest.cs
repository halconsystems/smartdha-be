using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Models;
public class SmartPayUploadBillRequest
{
    public string Consumer_Number { get; set; } = default!;
    public string Consumer_Detail { get; set; } = default!;
    public string Billing_Month { get; set; } = default!;
    public string DueDate { get; set; } = default!;
    public string ExpDate { get; set; } = default!;
    public decimal Amount { get; set; } 
    public decimal LateFee { get; set; }    
    public string CellNo { get; set; } = default!;
    public string EMail { get; set; } = default!;
    public string ReferenceInfo { get; set; } = default!;
}
public class SmartPayUploadBillResponse
{
    public string ResponseCode { get; set; } = default!;
    public string ResponseMsg { get; set; } = default!;
}

//SmartPayConsumerInquiryResponse 
public class SmartPayConsumerInquiryResponse
{
    public string ResponseCode { get; set; } = default!;
    public string ResponseMsg { get; set; } = default!;
    public List<SmartPayConsumerInquiryBill> Bills { get; set; } = new();
}
public class SmartPayConsumerInquiryBill
{
    public string Institution { get; set; } = default!;
    public string Consumer_Number { get; set; } = default!;
    public string Consumer_Detail { get; set; } = default!;
    public string Reference_Info { get; set; } = default!;
}

//SmartPayBillInquiryResponse 
public class SmartPayBillInquiryResponse
{
    public string ResponseCode { get; set; } = default!;
    public string ResponseMsg { get; set; } = default!;
    public SmartPayBillData BillData { get; set; } = default!;
}
public class SmartPayBillData
{
    public string Consumer_Number { get; set; } = default!;
    public string Consumer_Detail { get; set; } = default!;
    public string Reference_Info { get; set; } = default!;
    public string Institution { get; set; } = default!;
    public string Billing_Month { get; set; } = default!;

    public string BillAmount { get; set; } = default!;          // FIXED
    public string LateFee { get; set; } = default!;             // FIXED
    public string DueDate { get; set; } = default!;             // FIXED (string)
    public string ExpDate { get; set; } = default!;             // FIXED (string)
    public string AmountAfterDueDate { get; set; } = default!;  // FIXED

    public string PaymentStatus { get; set; } = default!;

    public string? Amount_Paid { get; set; }                    // FIXED
    public string? PaymentDateTime { get; set; }                // FIXED

    public string AuthNo { get; set; } = default!;
    public string Fee_Amount { get; set; } = default!;          // FIXED
    public string BillGenerateOn { get; set; } = default!;      // FIXED
}


//SmartPayHistory Detail
public class SmartPayConsumerHistoryResponse
{
    public string ResponseCode { get; set; } = default!;
    public string ResponseMsg { get; set; } = default!;
    public List<SmartPayConsumerHistoryBill> BillData { get; set; } = new();
}
public class SmartPayConsumerHistoryBill
{
    public string BillId { get; set; } = default!;
    public string Consumer_Number { get; set; } = default!;
    public string Consumer_Detail { get; set; } = default!;
    public string Reference_Info { get; set; } = default!;
    public string BillStatus { get; set; } = default!;
    public string DueDate1 { get; set; } = default!;
    public string ExpDate { get; set; } = default!;
    public string Amount { get; set; } = default!;              // FIXED
    public string Billing_Month { get; set; } = default!;
    public string? Date_Paid { get; set; }                      // FIXED nullable
    public string? Pay_Time { get; set; }                       // FIXED nullable
    public string? Amount_Paid { get; set; }                    // FIXED
    public string? Tran_Auth_Id { get; set; }                   // FIXED
    public string BillGenerateOn { get; set; } = default!;
}


public class SmartPayOptions
{
    public string BaseUrl { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
}
