using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class PayFastService : IPayFastService
{
    private readonly HttpClient _httpClient;

    public PayFastService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PayFastTokenResponse> GetAccessTokenAsync(
        PayFastTokenRequest request,
        CancellationToken ct)
    {
        var form = new Dictionary<string, string>
        {
            ["MERCHANT_ID"] = request.MerchantId,
            ["SECURED_KEY"] = request.SecuredKey,
            ["BASKET_ID"] = request.BasketId,
            ["TXNAMT"] = request.TransactionAmount.ToString("0.00"),
            ["CURRENCY_CODE"] = request.CurrencyCode
        };

        using var content = new FormUrlEncodedContent(form);

        var response = await _httpClient.PostAsync(
            "/api/Transaction/GetAccessToken",
            content,
            ct);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);

        return JsonSerializer.Deserialize<PayFastTokenResponse>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
}

