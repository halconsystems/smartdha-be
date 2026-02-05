using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PaymentIpn.Queries.InitiatePayFastCheckout;
public class PayFastCheckoutResponse
{
    public string AccessToken { get; set; } = default!;
    public string MainURL { get; set; } = "https://epg.apps.net.pk/api/Transaction/PostTransaction";
    public string BasketId { get; set; } = default!;
    public string TotalAmount { get; set; } = default!;
    public string MarchantId { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string CustomerMobileNo { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public string CheckoutURL { get; set; } = "https://gw.dhakarachi.org/api/paymentipn/checkout'";
    public string SuccessURL { get; set; } = "https://gw.dhakarachi.org/api/paymentipn/success";
    public string FailureURL { get; set; } = "'https://gw.dhakarachi.org/api/paymentipn/failure"!;
}

