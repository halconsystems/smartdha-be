using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
using DHAFacilitationAPIs.Domain.Enums.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.PaymentIpn.Queries.InitiatePayFastCheckout;
public record InitiatePayFastCheckoutCommand(
    string BillId
) : IRequest<ApiResult<PayFastCheckoutResponse>>;
public class InitiatePayFastCheckoutHandler
    : IRequestHandler<InitiatePayFastCheckoutCommand, ApiResult<PayFastCheckoutResponse>>
{
    private readonly IPaymentDbContext _db;
    private readonly IPayFastService _payFastService;
    private readonly ISecureKeyProtector _secureKeyProtector;
    private readonly ILateFeePolicyResolver _lateFeePolicyResolver;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISmartPayService _smartPayService;
    private readonly IPaymentBillService _paymentBillService;
    public InitiatePayFastCheckoutHandler(
        IPaymentDbContext db,
        IPayFastService payFastService,
        ISecureKeyProtector secureKeyProtector,
        ILateFeePolicyResolver lateFeePolicyResolver,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager,
        ISmartPayService smartPayService,
        IPaymentBillService paymentBillService)
    {
        _db = db;
        _payFastService = payFastService;
        _secureKeyProtector = secureKeyProtector;
        _lateFeePolicyResolver = lateFeePolicyResolver;
        _currentUserService = currentUserService;
        _userManager = userManager;
        _smartPayService = smartPayService;
        _paymentBillService = paymentBillService;
    }

    public async Task<ApiResult<PayFastCheckoutResponse>> Handle(
        InitiatePayFastCheckoutCommand request,
        CancellationToken ct)
    {
        var currentUserId = _currentUserService.UserId.ToString();
        if (string.IsNullOrEmpty(currentUserId))
            throw new UnAuthorizedException("Invalid user context.");

        var user = await _userManager.FindByIdAsync(currentUserId);

        if (user == null)
            throw new Exception("User not found.");

        var bill = await _db.PayBills
            .FirstOrDefaultAsync(x => x.SourceVoucherNo == request.BillId && x.PaymentStatus==PaymentBillStatus.Generated, ct);
        
        if (bill == null)
        {
            var billInquiry = await _smartPayService.BillInquiryAsync(
                request.BillId,
                ct
            );

            if (billInquiry?.BillData == null)
                throw new NotFoundException("Bill not found.");
            else
            {
                var billData = billInquiry.BillData;

                // 🔒 Basic validation
                if (string.IsNullOrWhiteSpace(billData.Consumer_Number))
                    throw new InvalidOperationException("Invalid SmartPay consumer number.");

                var smartPayRequest = new CreateSmartPayBillRequest
                {
                    // 🔹 SmartPay identifiers
                    ConsumerNumber = billData.Consumer_Number,
                    ConsumerDetail = billData.Consumer_Detail,
                    ReferenceInfo = billData.Reference_Info,
                    Institution = billData.Institution,

                    // 🔹 Amounts (SmartPay always sends strings)
                    BillAmount = billData.BillAmount,
                    LateFee = billData.LateFee ?? "0",
                    AmountAfterDueDate = billData.AmountAfterDueDate,

                    // 🔹 Dates (SmartPay format: dd/MM/yyyy)
                    DueDate = billData.DueDate,
                    ExpiryDate = billData.ExpDate,
                    BillGenerateOn = billData.BillGenerateOn,

                    // 🔹 Status (for reference only)
                    PaymentStatus = billData.PaymentStatus,

                    // 🔹 System user mapping
                    UserId = currentUserId   // resolved user in your system
                };

                bill = await _paymentBillService
                     .CreatePaymentBillFromSmartPayAsync(smartPayRequest,ct);
            }  
        }
        var now = DateTime.Now;

        // 1️⃣ Resolve late-fee policy per bill
        var policy = await _lateFeePolicyResolver
            .ResolveAsync(bill.SourceSystem, ct);

        // 2️⃣ Calculate FINAL payable amount
        var finalAmount = CalculateFinalPayableAmount(bill, policy, now);

        // 3️⃣ Safety check
        if (finalAmount <= 0)
            throw new InvalidOperationException("Invalid payable amount.");

        var merchant = await _db.PayMerchants
            .FirstAsync(x => x.Code == bill.MerchantCode && x.IsActive==true, ct);

        //var secureKey = _secureKeyProtector
        //    .Decrypt(merchant.SecureKeyEncrypted);

        var secureKey = "wZLV_F2VcDmJ9voSHp6t8ND";

        var basketId = Random.Shared.Next(100000, 999999).ToString();

        // 4️⃣ INSERT TRANSACTION (🔥 REQUIRED)
        var transaction = new PayTransaction
        {
            PayBillId = bill.PaymentBillId,
            BasketId = basketId,
            AttemptAmount = finalAmount,
            Status = PaymentTransactionStatus.Initiated,
            InitiatedAt = DateTime.Now
        };

        _db.PayTransactions.Add(transaction);
        await _db.SaveChangesAsync(ct);

        // 5️⃣ Call PayFast
        var tokenResponse = await _payFastService.GetAccessTokenAsync(
            new PayFastTokenRequest
            {
                MerchantId = merchant.MerchantId.ToString(),
                SecuredKey = secureKey,
                BasketId = basketId,
                TransactionAmount = finalAmount
            },
            ct);

        // 6️⃣ Update transaction after token
        transaction.Status = PaymentTransactionStatus.TokenIssued;
        await _db.SaveChangesAsync(ct);

        return ApiResult<PayFastCheckoutResponse>.Ok(new PayFastCheckoutResponse
        {
            AccessToken = tokenResponse.AccessToken,
            BasketId = basketId,
            TotalAmount = finalAmount.ToString("0.##"),
            MarchantId = merchant.MerchantId.ToString(),
            DisplayName = merchant.DisplayName,
            CustomerEmail = user.Email ?? string.Empty,
            CustomerMobileNo =
                user.RegisteredMobileNo?.StartsWith("92") == true
                    ? "0" + user.RegisteredMobileNo.Substring(2)
                    : user.RegisteredMobileNo ?? string.Empty
        });
    }

    private static decimal CalculateFinalPayableAmount(
    PayBill bill,
    PayLateFeePolicy policy,
    DateTime nowUtc)
    {
        if (!bill.DueDate.HasValue)
            return bill.OutstandingAmount;

        var lateStartDate = bill.DueDate.Value.AddDays(policy.GraceDays);

        if (nowUtc <= lateStartDate)
            return bill.OutstandingAmount;

        var lateFee = policy.LateFeeType switch
        {
            LateFeeType.Fixed => policy.FixedLateFee,

            LateFeeType.PerDay =>
                (decimal)(nowUtc.Date - lateStartDate.Date).TotalDays
                * policy.PerDayLateFee,

            _ => 0
        };

        return bill.OutstandingAmount + lateFee;
    }

}

