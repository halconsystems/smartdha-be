using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

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

        return entity.Id;
    }
}

