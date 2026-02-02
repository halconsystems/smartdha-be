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
        // Prevent duplicate snapshot
        var exists = await _paymentDb.PayBills.AnyAsync(x =>
            x.SourceSystem == r.SourceSystem &&
            x.SourceVoucherId == r.SourceVoucherId,
            ct);

        if (exists)
            throw new InvalidOperationException("Payment bill already exists.");

        // Resolve merchant ONCE
        var merchantCode = await _merchantResolver.ResolveAsync(
            r.SourceSystem,
            r.EntityType,
            r.EntityId,
            ct);

        var bill = new PayBill
        {
            Id = Guid.NewGuid(),
            SourceSystem = r.SourceSystem,
            SourceVoucherId = r.SourceVoucherId,
            SourceVoucherNo = r.SourceVoucherNo,
            Title = r.Title,

            EntityType = r.EntityType,
            EntityId = r.EntityId,
            EntityName = r.EntityName,

            UserId = r.UserId,
            MerchantCode = merchantCode,

            TotalAmount = r.TotalAmount,
            PaidAmount = 0,

            Status = PaymentBillStatus.Generated,
            DueDate = r.DueDate,
            CreatedAt = DateTime.Now
        };

        _paymentDb.PayBills.Add(bill);
        await _paymentDb.SaveChangesAsync(ct);

        return bill.Id;
    }
}
