using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PaymentIpn.Commands.SavePaymentIpn;
public class PaymentIpnRequestDto
{
    public string? err_code { get; set; }
    public string? err_msg { get; set; }

    public string? basket_id { get; set; }
    public string? transaction_id { get; set; }

    public string? responseKey { get; set; }
    public string? Response_Key { get; set; }
    public string? validation_hash { get; set; }

    public string? order_date { get; set; }

    public string? amount { get; set; }
    public string? transaction_amount { get; set; }
    public string? merchant_amount { get; set; }
    public string? discounted_amount { get; set; }

    public string? transaction_currency { get; set; }
    public string? PaymentName { get; set; }

    public string? issuer_name { get; set; }
    public string? masked_pan { get; set; }

    public string? mobile_no { get; set; }
    public string? email_address { get; set; }

    public string? is_international { get; set; }
    public string? recurring_txn { get; set; }

    public string? bill_number { get; set; }
    public string? customer_id { get; set; }

    public string? rdv_message_key { get; set; }
    public string? additional_value { get; set; }
}

