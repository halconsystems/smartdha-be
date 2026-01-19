using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.OrderDispatch;
using DHAFacilitationAPIs.Application.Feature.Panic;
using DHAFacilitationAPIs.Application.ViewModels;

namespace MobileAPI.Services;


public class OrderRealtimeMobileAdapter : IOrderRealTime
{
    private readonly HttpClient _http;
    private readonly string _secret;

    public OrderRealtimeMobileAdapter(HttpClient http, IConfiguration cfg)
    {
        _http = http; // BaseAddress is set via DI above
        _secret = cfg["Realtime:SharedSecret"]
            ?? throw new InvalidOperationException("Missing Realtime:SharedSecret");
    }

    //public async Task PanicCreatedAsync(PanicCreatedRealtimeDto dto)
    //{
    //    using var req = new HttpRequestMessage(HttpMethod.Post, "/internal/realtime/panic-created")
    //    {
    //        Content = JsonContent.Create(dto) // System.Text.Json, camelCase by default
    //    };
    //    req.Headers.Add("X-RT-Secret", _secret);

    //    var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
    //    if (!res.IsSuccessStatusCode)
    //    {
    //        var body = await res.Content.ReadAsStringAsync();
    //        throw new InvalidOperationException(
    //            $"Realtime forward failed: {(int)res.StatusCode} {res.ReasonPhrase}. Body: {body}");
    //    }
    //}

    public async Task OrderCreatedAsync(OrderCreatedRealtimeDto dto)
    {
        const string path = "/internal/realtime/order-created";

        using var req = new HttpRequestMessage(HttpMethod.Post, path)
        { Content = JsonContent.Create(dto) };
        req.Headers.Add("X-RT-Secret", _secret);

        using var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        var responseText = await res.Content.ReadAsStringAsync();

        // write current response to wwwroot/realtime-response.txt
        var root = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        //Directory.CreateDirectory(root);
        await File.WriteAllTextAsync(Path.Combine(root, "realtime-response.txt"), responseText ?? "");

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"Realtime forward failed: {(int)res.StatusCode} {res.ReasonPhrase}.");
    }

    public async Task OrderUpdateAsync(OrderCreatedRealtimeDto dto)
    {
        const string path = "/internal/realtime/order-update";

        using var req = new HttpRequestMessage(HttpMethod.Post, path)
        { Content = JsonContent.Create(dto) };
        req.Headers.Add("X-RT-Secret", _secret);

        using var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        var responseText = await res.Content.ReadAsStringAsync();

        // write current response to wwwroot/realtime-response.txt
        var root = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        //Directory.CreateDirectory(root);
        await File.WriteAllTextAsync(Path.Combine(root, "realtime-response.txt"), responseText ?? "");

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"Realtime forward failed: {(int)res.StatusCode} {res.ReasonPhrase}.");
    }

    public async Task SendOrderUpdatedAsync(OrderUpdateRealTimeDTO dto, CancellationToken ct)
    {
        const string path = "/internal/realtime/order-statusupdate";

        using var req = new HttpRequestMessage(HttpMethod.Post, path)
        { Content = JsonContent.Create(dto) };
        req.Headers.Add("X-RT-Secret", _secret);

        using var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        var responseText = await res.Content.ReadAsStringAsync();

        // write current response to wwwroot/realtime-response.txt
        var root = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        //Directory.CreateDirectory(root);
        await File.WriteAllTextAsync(Path.Combine(root, "realtime-response.txt"), responseText ?? "");

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"Realtime forward failed: {(int)res.StatusCode} {res.ReasonPhrase}.");
    }
    public async Task UpdateLocationAsync(UpdateLocation dto)
    {
        const string path = "/internal/realtime/vehicle-locationupdate";

        using var req = new HttpRequestMessage(HttpMethod.Post, path)
        { Content = JsonContent.Create(dto) };
        req.Headers.Add("X-RT-Secret", _secret);

        using var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        var responseText = await res.Content.ReadAsStringAsync();

        // write current response to wwwroot/realtime-response.txt
        var root = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        //Directory.CreateDirectory(root);
        await File.WriteAllTextAsync(Path.Combine(root, "realtime-response.txt"), responseText ?? "");

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"Realtime forward failed: {(int)res.StatusCode} {res.ReasonPhrase}.");
    }
}

