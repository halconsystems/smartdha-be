using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class PaymentIpnLog : BaseAuditableEntity
{
    public string? ErrCode { get; set; }
    public string? ErrMsg { get; set; }

    public string? BasketId { get; set; }
    public string? TransactionId { get; set; }

    public string? ResponseKey { get; set; }
    public string? ValidationHash { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? Amount { get; set; }
    public decimal? TransactionAmount { get; set; }
    public decimal? MerchantAmount { get; set; }
    public decimal? DiscountedAmount { get; set; }

    public string? TransactionCurrency { get; set; }
    public string? PaymentName { get; set; }

    public string? IssuerName { get; set; }
    public string? MaskedPan { get; set; }

    public string? MobileNo { get; set; }
    public string? EmailAddress { get; set; }

    public bool? IsInternational { get; set; }
    public bool? RecurringTxn { get; set; }

    public string? BillNumber { get; set; }
    public string? CustomerId { get; set; }

    public string? RdvMessageKey { get; set; }
    public string? AdditionalValue { get; set; }

    // 🔐 Raw IPN (VERY IMPORTANT for audit/debug)
    public string RawPayload { get; set; } = default!;
}

