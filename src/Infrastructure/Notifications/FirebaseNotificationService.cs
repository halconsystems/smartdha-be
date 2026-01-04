using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Common.Settings;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Notifications;
public class FirebaseNotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly FirebaseOptions _options;
    private readonly IFirebaseTokenProvider _tokenProvider;
    private readonly IApplicationDbContext _context;

    public FirebaseNotificationService(
        HttpClient httpClient,
        IOptions<FirebaseOptions> options,
        IFirebaseTokenProvider tokenProvider,
        IApplicationDbContext context)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _tokenProvider = tokenProvider;
        _context = context;
    }

    public async Task<FirebaseResponse> SendFirebaseNotificationAsync(
        string deviceToken,
        string title,
        string body,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        var fcmToken = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
        var url = "https://fcm.googleapis.com/v1/projects/smart-dha-security/messages:send";
     
        
        var payload = new
        {
            message = new
            {
                token = deviceToken,
                // STATIC ANDROID CHANNEL + SOUND + VISIBILITY
                android = new
                {
                    priority = "high",
                    notification = new
                    {
                        channel_id = "emergency_channel",
                        sound = "alarm",
                        visibility = "public",
                        tag = "alert"
                    }
                },
                notification = new
                {
                    title = title,
                    body = body
                },
                data = data
            }
        };

        string json = JsonSerializer.Serialize(payload);

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        requestMessage.Headers.Authorization =
        new AuthenticationHeaderValue("Bearer", fcmToken);
        //requestMessage.Headers.Authorization =
        //    new AuthenticationHeaderValue("key", "=" + _options.ServerKey);

        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = null!;
        string? responseText = null;
        bool isSuccess = false;

        try
        {
            response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            responseText = await response.Content.ReadAsStringAsync(cancellationToken);
            isSuccess = response.IsSuccessStatusCode;
        }
        finally
        {
            // Always log request/response
            try
            {
                var log = new FirebaseApiLog
                {
                    DeviceToken = deviceToken,
                    Title = title,
                    Body = body,
                    PayloadJson = json,
                    ResponseJson = responseText,
                    StatusCode = (int)(response?.StatusCode ?? 0),
                    IsSuccess = isSuccess
                };

                _context.FirebaseApiLogs.Add(log);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Never throw from logging
                // Optionally: _logger.LogError(ex, "Firebase logging failed");
            }
        }

        if (!isSuccess)
        {
            return new FirebaseResponse
            {
                IsSuccess = false,
                StatusCode = response.StatusCode,
                ErrorMessage = responseText
            };
        }

        return new FirebaseResponse
        {
            IsSuccess = true,
            StatusCode = response.StatusCode,
            ErrorMessage = null
        };

    }
}
