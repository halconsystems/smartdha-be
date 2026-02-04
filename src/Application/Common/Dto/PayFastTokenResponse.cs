using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Dto;
public class PayFastTokenResponse
{
    [JsonPropertyName("ACCESS_TOKEN")]
    public string AccessToken { get; set; } = default!;

    [JsonPropertyName("MERCHANT_ID")]
    public decimal MerchantId { get; set; }

    [JsonPropertyName("NAME")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("GENERATED_DATE_TIME")]
    public DateTimeOffset GeneratedDateTime { get; set; }
}

