using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Dto;
public class PayFastTokenResponse
{
    public string AccessToken { get; set; } = default!;
    public string RedirectUrl { get; set; } = default!;
    public string BasketId { get; set; } = default!;
    public string ResponseCode { get; set; } = default!;
    public string ResponseMessage { get; set; } = default!;
}

