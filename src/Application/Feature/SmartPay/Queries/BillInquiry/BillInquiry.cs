using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.SmartPay.Queries.BillInquiry;
public class BillInquiryQuery : IRequest<SmartPayBillInquiryResponse>
{
    public string ConsumerNo { get; set; } = default!;
}
public class BillInquiryQueryHandler :
        IRequestHandler<BillInquiryQuery, SmartPayBillInquiryResponse>
{
   private readonly ISmartPayService _smartPayService;
   private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public BillInquiryQueryHandler(ISmartPayService smartPayService, ICurrentUserService currentUserService,UserManager<ApplicationUser> userManager)
    {
        _smartPayService = smartPayService;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<SmartPayBillInquiryResponse> Handle(
        BillInquiryQuery request,
        CancellationToken cancellationToken)
    {
        //if (string.IsNullOrWhiteSpace(request.ConsumerNo))
        //    throw new ArgumentException("Consumer number is required.");


        //if (string.IsNullOrWhiteSpace(request.ConsumerNo))
        //    throw new ArgumentException("Consumer number is required.");



        //return await _smartPayService.BillInquiryAsync(
        //    request.ConsumerNo,
        //    cancellationToken);

        var bills = new Dictionary<string, SmartPayBillData>
        {
            ["1001"] = new SmartPayBillData
            {
                Consumer_Number = "1001",
                Consumer_Detail = "Security Charges - Phase 2",
                Reference_Info = "SC-2025-0001",
                Institution = "DHA Karachi",
                Billing_Month = "JAN-2025",

                BillAmount = "10",
                LateFee = "500",
                DueDate = "2025-01-20",
                ExpDate = "2025-01-31",
                AmountAfterDueDate = "510",

                PaymentStatus = "UNPAID",
                Amount_Paid = null,
                PaymentDateTime = null,

                AuthNo = "",
                Fee_Amount = "0",
                BillGenerateOn = "2025-01-01"
            },

            ["1002"] = new SmartPayBillData
            {
                Consumer_Number = "1002",
                Consumer_Detail = "Maintenance Charges - Phase 7",
                Reference_Info = "MNT-2025-0042",
                Institution = "DHA Karachi",
                Billing_Month = "JAN-2025",

                BillAmount = "10",
                LateFee = "1000",
                DueDate = "2025-01-25",
                ExpDate = "2025-02-05",
                AmountAfterDueDate = "1010",

                PaymentStatus = "UNPAID",
                Amount_Paid = null,
                PaymentDateTime = null,

                AuthNo = "",
                Fee_Amount = "0",
                BillGenerateOn = "2025-01-05"
            },

            ["1003"] = new SmartPayBillData
            {
                Consumer_Number = "1003",
                Consumer_Detail = "DA Club",
                Reference_Info = "DA-2025-0110",
                Institution = "DHA Karachi",
                Billing_Month = "JAN-2025",

                BillAmount = "10",
                LateFee = "300",
                DueDate = "2025-01-15",
                ExpDate = "2025-01-25",
                AmountAfterDueDate = "310",

                PaymentStatus = "PAID",
                Amount_Paid = "10",
                PaymentDateTime = "2025-01-10 11:30",

                AuthNo = "AUTH-998877",
                Fee_Amount = "0",
                BillGenerateOn = "2025-01-01"
            }
        };

        if (!bills.TryGetValue(request.ConsumerNo, out var billData))
        {
            return await Task.FromResult(new SmartPayBillInquiryResponse
            {
                ResponseCode = "01",
                ResponseMsg = "Bill not found",
                BillData = null!
            });
        }

        return await Task.FromResult(new SmartPayBillInquiryResponse
        {
            ResponseCode = "00",
            ResponseMsg = "SUCCESS",
            BillData = billData
        });
    }
}
