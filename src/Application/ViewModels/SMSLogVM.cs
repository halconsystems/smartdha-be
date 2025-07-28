using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.ViewModels;
public class SMSLogVM
{
    public string STATUS { get; set; } = string.Empty;

    public string MESSAGE_ID { get; set; } = string.Empty;

    public string COUNTRY_SUPPORTED { get; set; } = string.Empty;

    public string COUNTRY_CODE { get; set; } = string.Empty;

    public string COUNTRY_ISO { get; set; } = string.Empty;

    public string NETWORK_NAME { get; set; } = string.Empty;

    public string RECEIVER_NUMBER { get; set; } = string.Empty;

    public string ERROR_FILTER { get; set; } = string.Empty;

    public string ERROR_CODE { get; set; } = string.Empty;

    public string ERROR_DESCRIPTION { get; set; } = string.Empty;

    public string CHARGED_BALANCE { get; set; } = string.Empty;

    public string CHARGING_UNIT { get; set; } = string.Empty;
}
