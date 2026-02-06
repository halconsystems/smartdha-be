using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Dto;
public class PayFastTokenRequest
{
    public string MerchantId { get; set; } = default!;
    public string SecuredKey { get; set; } = default!;
    public string BasketId { get; set; } = default!;
    public string TransactionAmount { get; set; } = default!;
    public string CurrencyCode { get; set; } = "PKR";
}

