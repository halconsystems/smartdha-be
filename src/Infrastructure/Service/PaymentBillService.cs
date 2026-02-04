using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
using DHAFacilitationAPIs.Domain.Enums.Payment;
using DHAFacilitationAPIs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class PaymentBillService : IPaymentBillService
{
    private readonly IPaymentDbContext _paymentDb;
    private readonly IMerchantResolver _merchantResolver;
    private readonly ILateFeePolicyResolver _lateFeePolicyResolver;

    public PaymentBillService(
        IPaymentDbContext paymentDb,
        IMerchantResolver merchantResolver,
        ILateFeePolicyResolver lateFeePolicyResolver)
    {
        _paymentDb = paymentDb;
        _merchantResolver = merchantResolver;
        _lateFeePolicyResolver = lateFeePolicyResolver;
    }

    public async Task<Guid> CreatePaymentBillAsync(
     CreatePaymentBillRequest r,
     CancellationToken ct)
    {
        // =========================
        // 1️⃣ Prevent duplicate snapshot
        // =========================
        var exists = await _paymentDb.PayBills.AnyAsync(x =>
            x.SourceSystem == r.SourceSystem &&
            x.SourceVoucherId == r.SourceVoucherId,
            ct);

        if (exists)
            throw new InvalidOperationException(
                $"Payment bill already exists for {r.SourceSystem} voucher {r.SourceVoucherId}");

        // =========================
        // 2️⃣ Resolve merchant ONCE
        // =========================
        var merchantCode = await _merchantResolver.ResolveAsync(
            r.SourceSystem,
            r.EntityType,
            r.EntityId,
            ct);

        // =========================
        // 3️⃣ Create Payment Bill
        // =========================

        // Resolve late fee policy
        var policy = await _lateFeePolicyResolver
            .ResolveAsync(r.SourceSystem, ct);

        var now = DateTime.Now;

        // 1️⃣ Due Date
        DateTime dueDate = r.DueDate
            ?? now.AddDays(policy.DueAfterDays);

        // 2️⃣ Expiry Date
        DateTime expiryDate = policy.ExpireAfterDays > 0
            ? dueDate.AddDays(policy.ExpireAfterDays)
            : dueDate.AddYears(10); // never expire (fallback)


        var bill = new PayBill
        {
            PaymentBillId = Guid.NewGuid(),

            // 🔹 Consumer / Owner
            UserId = r.UserId,
            EntityName = r.EntityName,
            SourceSystem = r.SourceSystem,

            // 🔹 Bill Identity
            SourceVoucherId = r.SourceVoucherId,
            SourceVoucherNo = r.SourceVoucherNo,
            Title = r.Title,

            // 🔹 Billing Info
            BillAmount = r.TotalAmount,
            PaidAmount = 0,
            OutstandingAmount = r.TotalAmount,

            DueDate = dueDate,
            ExpiryDate = expiryDate,

            // 🔹 Status
            PaymentStatus = PaymentBillStatus.Generated,

            // 🔹 Payment Info
            MerchantCode = merchantCode,
            LastPaymentDate = null,
            LastAuthNo = null,

            // 🔹 Audit
            BillGeneratedOn = DateTime.Now,
            IsSmartPayBill=false
        };

        _paymentDb.PayBills.Add(bill);
        await _paymentDb.SaveChangesAsync(ct);

        return bill.PaymentBillId;
    }

    // ===============================
    // 🔥 SMARTPAY METHOD (NEW)
    // ===============================
    public async Task<PayBill> CreatePaymentBillFromSmartPayAsync(
        CreateSmartPayBillRequest r,
        CancellationToken ct)
    {
        // 1️⃣ Validate consumer number
        if (string.IsNullOrWhiteSpace(r.ConsumerNumber) || r.ConsumerNumber.Length < 4)
            throw new InvalidOperationException("Invalid SmartPay consumer number.");

        // 2️⃣ Prevent duplicates (SmartPay reference)
        var exists = await _paymentDb.PayBills.AnyAsync(x =>
            x.SourceVoucherNo == r.ConsumerNumber,
            ct);

        if (exists)
            throw new InvalidOperationException("SmartPay bill already exists.");

        // 3️⃣ Extract SmartPay merchant prefix (first 4 digits)
        var smartPayMerchantId = r.ConsumerNumber.Substring(0, 4);

        // 4️⃣ Resolve MerchantCode
        var merchantCode = await _merchantResolver
            .ResolveBySmartPayCodeAsync(smartPayMerchantId, ct);

        // 5️⃣ Parse amounts safely
        var billAmount = decimal.Parse(
            r.AmountAfterDueDate,
            CultureInfo.InvariantCulture);

        // 6️⃣ Parse dates
        var dueDate = DateTime.ParseExact(
            r.DueDate,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture);

        var expiryDate = DateTime.ParseExact(
            r.ExpiryDate,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture);

        var generatedOn = DateTime.ParseExact(
            r.BillGenerateOn,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture);

        // 7️⃣ Create PayBill
        var bill = new PayBill
        {
            PaymentBillId = Guid.NewGuid(),

            UserId = r.UserId,
            EntityName = r.ConsumerDetail,

            SourceSystem = r.ReferenceInfo,
            SourceVoucherId = Guid.NewGuid(),
            SourceVoucherNo = r.ConsumerNumber,

            Title = r.Institution,

            BillAmount = billAmount,
            PaidAmount = 0,
            OutstandingAmount = billAmount,

            DueDate = dueDate,
            ExpiryDate = expiryDate,

            PaymentStatus = PaymentBillStatus.Generated,

            MerchantCode = merchantCode,

            BillGeneratedOn = generatedOn,
            IsSmartPayBill=true
        };

        _paymentDb.PayBills.Add(bill);
        await _paymentDb.SaveChangesAsync(ct);

        return bill;
    }
}
