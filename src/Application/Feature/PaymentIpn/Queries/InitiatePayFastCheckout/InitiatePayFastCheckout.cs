using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
using DHAFacilitationAPIs.Domain.Enums.Payment;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.PaymentIpn.Queries.InitiatePayFastCheckout;
public record InitiatePayFastCheckoutCommand(
    Guid BillId
) : IRequest<ApiResult<PayFastCheckoutResponse>>;
public class InitiatePayFastCheckoutHandler
    : IRequestHandler<InitiatePayFastCheckoutCommand, ApiResult<PayFastCheckoutResponse>>
{
    private readonly IPaymentDbContext _db;
    private readonly IPayFastService _payFastService;
    private readonly ISecureKeyProtector _secureKeyProtector;
    private readonly ILateFeePolicyResolver _lateFeePolicyResolver;

    public InitiatePayFastCheckoutHandler(
        IPaymentDbContext db,
        IPayFastService payFastService,
        ISecureKeyProtector secureKeyProtector,
        ILateFeePolicyResolver lateFeePolicyResolver)
    {
        _db = db;
        _payFastService = payFastService;
        _secureKeyProtector = secureKeyProtector;
        _lateFeePolicyResolver = lateFeePolicyResolver;
    }

    public async Task<ApiResult<PayFastCheckoutResponse>> Handle(
        InitiatePayFastCheckoutCommand request,
        CancellationToken ct)
    {
        var bill = await _db.PayBills
            .FirstOrDefaultAsync(x => x.SourceVoucherId == request.BillId, ct);

        if (bill == null)
            return ApiResult<PayFastCheckoutResponse>.Fail("Invalid bill");


        var now = DateTime.UtcNow;

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

        var secureKey = _secureKeyProtector
            .Decrypt(merchant.SecureKeyEncrypted);

        var basketId = Random.Shared.Next(100000, 999999).ToString();

        var tokenResponse = await _payFastService.GetAccessTokenAsync(
            new PayFastTokenRequest
            {
                MerchantId = merchant.MerchantId.ToString(),
                SecuredKey = secureKey,
                BasketId = basketId,
                TransactionAmount = decimal.Parse(
                 finalAmount.ToString("0.##"),
                 CultureInfo.InvariantCulture)
            },
            ct);

        return ApiResult<PayFastCheckoutResponse>.Ok(new PayFastCheckoutResponse
        {
            AccessToken = tokenResponse.AccessToken,
            RedirectUrl = "https://epg.apps.net.pk/api/Transaction/PostTransaction",
            BasketId = basketId,
            TotalAmount = finalAmount.ToString("0.##")
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

