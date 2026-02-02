using System;
using System.Collections.Generic;
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

    public PaymentBillService(
        IPaymentDbContext paymentDb,
        IMerchantResolver merchantResolver)
    {
        _paymentDb = paymentDb;
        _merchantResolver = merchantResolver;
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

            DueDate = r.DueDate,

            // Optional expiry (recommended)
            ExpiryDate = r.DueDate ?? DateTime.UtcNow.AddMinutes(15),

            // 🔹 Status
            PaymentStatus = PaymentBillStatus.Generated,

            // 🔹 Payment Info
            MerchantCode = merchantCode,
            LastPaymentDate = null,
            LastAuthNo = null,

            // 🔹 Audit
            BillGeneratedOn = DateTime.UtcNow
        };

        _paymentDb.PayBills.Add(bill);
        await _paymentDb.SaveChangesAsync(ct);

        return bill.PaymentBillId;
    }


}
