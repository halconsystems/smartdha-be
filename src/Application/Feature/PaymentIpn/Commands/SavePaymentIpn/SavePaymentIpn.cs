using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
using DHAFacilitationAPIs.Domain.Enums.Payment;

namespace DHAFacilitationAPIs.Application.Feature.PaymentIpn.Commands.SavePaymentIpn;
public record SavePaymentIpnCommand(
    PaymentIpnRequestDto Payload,
    string RawPayload
) : IRequest<Guid>;
public class SavePaymentIpnCommandHandler
    : IRequestHandler<SavePaymentIpnCommand, Guid>
{
    private readonly IPaymentDbContext _ctx;

    public SavePaymentIpnCommandHandler(IPaymentDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Guid> Handle(SavePaymentIpnCommand request, CancellationToken ct)
    {
        var p = request.Payload;

        DateTime? orderDate = null;
        if (DateTime.TryParse(p.order_date, out var dt))
            orderDate = dt;

        decimal? ParseDecimal(string? s)
            => decimal.TryParse(s, out var d) ? d : null;

        bool? ParseBool(string? s)
            => bool.TryParse(s, out var b) ? b : null;

        var entity = new PaymentIpnLog
        {
            Id = Guid.NewGuid(),

            ErrCode = p.err_code,
            ErrMsg = p.err_msg,

            BasketId = p.basket_id,
            TransactionId = p.transaction_id,

            ResponseKey = p.responseKey ?? p.Response_Key,
            ValidationHash = p.validation_hash,

            OrderDate = orderDate,

            Amount = ParseDecimal(p.amount),
            TransactionAmount = ParseDecimal(p.transaction_amount),
            MerchantAmount = ParseDecimal(p.merchant_amount),
            DiscountedAmount = ParseDecimal(p.discounted_amount),

            TransactionCurrency = p.transaction_currency,
            PaymentName = p.PaymentName,

            IssuerName = p.issuer_name,
            MaskedPan = p.masked_pan,

            MobileNo = p.mobile_no,
            EmailAddress = p.email_address,

            IsInternational = ParseBool(p.is_international),
            RecurringTxn = ParseBool(p.recurring_txn),

            BillNumber = p.bill_number,
            CustomerId = p.customer_id,

            RdvMessageKey = p.rdv_message_key,
            AdditionalValue = p.additional_value,

            RawPayload = request.RawPayload
        };

        _ctx.PaymentIpnLogs.Add(entity);
        await _ctx.SaveChangesAsync(ct);


        var transaction = await _ctx.Set<PayTransaction>()      
            .FirstOrDefaultAsync(x => x.BasketId == p.basket_id, ct);

        if (transaction == null)
        {
           return entity.Id;
        }

        // =========================
        // 2️⃣ Load bill
        // =========================
        var bill = await _ctx.Set<PayBill>()
            .FirstOrDefaultAsync(x => x.PaymentBillId == transaction.PayBillId, ct);

        if (bill == null)
        {
            return entity.Id;
        }

        // =========================
        // 3️⃣ Idempotency guard
        // =========================
        if (transaction.Status == PaymentTransactionStatus.Success)
        {
            // Already processed
            return entity.Id;
        }

        // =========================
        // 4️⃣ Success / Failure handling
        // =========================
        if (p.err_code == "000")
        {
            var paidAmount = ParseDecimal(p.transaction_amount);

            // ---- Update transaction
            transaction.Status = PaymentTransactionStatus.Success;
            transaction.GatewayTransactionId = p.transaction_id;
            transaction.CompletedAt = DateTime.Now;

            // ---- Update bill
            bill.PaidAmount += paidAmount ?? 0m;
            bill.OutstandingAmount = bill.BillAmount - bill.PaidAmount;

            bill.LastPaymentDate = DateTime.Now;
            bill.LastAuthNo = p.transaction_id;

            // ---- Bill status
            if (bill.OutstandingAmount <= 1)
            {
                bill.PaymentStatus = PaymentBillStatus.Paid;
                bill.OutstandingAmount = 0;
            }
            else
            {
                bill.PaymentStatus = PaymentBillStatus.PartiallyPaid;
            }
        }
        // =========================
        // 5️⃣ Save all changes
        // =========================
        await _ctx.SaveChangesAsync(ct);
        return entity.Id;
    }
}

