using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;
using ValidationException = FluentValidation.ValidationException;

namespace DHAFacilitationAPIs.Application.Feature.MyBills.Queries.GetAllBillsByConsumerId;
public record GetAllBillsByConsumerId(string ConsumerNo)
    : IRequest<ApiResult<SmartPayBillData>>;
public class GetAllBillsByConsumerIdHandler
    : IRequestHandler<GetAllBillsByConsumerId, ApiResult<SmartPayBillData>>
{
    private readonly ISmartPayService _smartPayService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public GetAllBillsByConsumerIdHandler(
        ISmartPayService smartPayService,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService)
    {
        _smartPayService = smartPayService;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResult<SmartPayBillData>> Handle(
        GetAllBillsByConsumerId request,
        CancellationToken ct)
    {
       
        // 🔹 Bill Inquiry for provided consumer number
        var billInquiry = await _smartPayService.BillInquiryAsync(
            request.ConsumerNo,
            ct);

        if (billInquiry?.BillData == null)
            throw new NotFoundException("Bill not found for this consumer.");

        var bill = billInquiry.BillData;

        if (bill.PaymentStatus != "Generated" &&
            bill.PaymentStatus != "Pending")
            throw new ValidationException("Bill is already paid.");

        var response = new SmartPayBillData
        {
            Consumer_Number = request.ConsumerNo,

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
        };

        return ApiResult<SmartPayBillData>.Ok(response);
    }
}

