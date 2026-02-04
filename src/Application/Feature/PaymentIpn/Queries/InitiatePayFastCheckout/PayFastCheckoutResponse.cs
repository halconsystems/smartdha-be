using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PaymentIpn.Queries.InitiatePayFastCheckout;
public class PayFastCheckoutResponse
{
    public string AccessToken { get; set; } = default!;
    public string RedirectUrl { get; set; } = default!;
    public string BasketId { get; set; } = default!;
    public string TotalAmount { get; set; } = default!;
    public string MarchantId { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}

