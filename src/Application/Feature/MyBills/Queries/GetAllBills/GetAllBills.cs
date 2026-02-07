using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetDirectorates;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.AllReservations;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetAllBills;
public record GetAllBillsQuery() : IRequest<ApiResult<List<SmartPayBillData>>>;
public class GetAllBillsHandler : IRequestHandler<GetAllBillsQuery, ApiResult<List<SmartPayBillData>>>
{
    private readonly ISmartPayService _smartPayService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPropertyInfoService _billConsumerService;
    public GetAllBillsHandler(ISmartPayService smartPayService, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService, IPropertyInfoService billConsumerService)
    {
        _smartPayService = smartPayService;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _billConsumerService = billConsumerService;
    }
    public async Task<ApiResult<List<SmartPayBillData>>> Handle(GetAllBillsQuery request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        var getuserInfo = await _userManager.Users
            .Where(u => u.Id == userId).FirstOrDefaultAsync(ct);

        if (getuserInfo == null)
            throw new NotFoundException("User not found.");

        if (string.IsNullOrEmpty(getuserInfo.RegisteredMobileNo))
            throw new NotFoundException("Registered MobileNo not found for the user.");

        // getuserInfo.RegisteredMobileNo = "923222781985"; // Temporary hardcoded for testing, replace with actual mobile number from user info
        var consumerInquiry = await _smartPayService.ConsumerInquiryAsync(
            getuserInfo.RegisteredMobileNo,
            ct);

        var result = new List<SmartPayBillData>();

        foreach (var consumer in consumerInquiry.Bills)
        {
            // Call bill inquiry for EACH consumer number
            var billInquiry = await _smartPayService.BillInquiryAsync(
                consumer.Consumer_Number,
                ct
            );

            if (billInquiry?.BillData == null)
                continue;

            var bill = billInquiry?.BillData;
            if (bill == null)
                continue;

            if (bill.PaymentStatus == "Generated" || bill.PaymentStatus == "Pending")
            {
                result.Add(new SmartPayBillData
                {
                    // 🔹 Consumer Info
                    Consumer_Number = consumer.Consumer_Number,
                    Consumer_Detail = consumer.Consumer_Detail,
                    Institution = consumer.Institution,

                    // 🔹 Bill Info
                    Reference_Info = bill.Reference_Info,
                    Billing_Month = bill.Billing_Month,

                    BillAmount = bill.BillAmount,
                    LateFee = bill.LateFee,
                    DueDate = bill.DueDate,
                    ExpDate = bill.ExpDate,
                    AmountAfterDueDate = bill.AmountAfterDueDate,

                    PaymentStatus = bill.PaymentStatus,
                    Amount_Paid = bill.Amount_Paid,
                    PaymentDateTime = bill.PaymentDateTime,

                    AuthNo = bill.AuthNo,
                    Fee_Amount = bill.Fee_Amount,
                    BillGenerateOn = bill.BillGenerateOn
                });
            }
        }

        var bills = await _billConsumerService
            .GetPendingBillsByUserAsync(userId, ct);

        if(bills != null && bills.Any())
            result.AddRange(bills);

        return ApiResult<List<SmartPayBillData>>.Ok(result);
    }
}
