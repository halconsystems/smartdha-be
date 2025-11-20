using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Infrastructure.Data;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Service;

 public class SmartPayService : ISmartPayService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly IApplicationDbContext _db;
        private readonly SmartPayOptions _options;
        private readonly ICurrentUserService _currentUser;
        private readonly JsonSerializerOptions _jsonOptions;

        public SmartPayService(
            ApplicationDbContext db,
            IOptions<SmartPayOptions> options,
            ICurrentUserService currentUser)
        {
            _db = db;
            _options = options.Value;
            _currentUser = currentUser;

            if (!_httpClient.DefaultRequestHeaders.Contains("APIKey"))
            {
                _httpClient.DefaultRequestHeaders.Add("APIKey", _options.ApiKey);
            }

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // ========= Helper: Logging =========
        private async Task LogAsync(DateTime req,DateTime responsedt, string apiName, object request, string? response, bool success,CancellationToken cancellationToken)
        {
            var log = new SmartPayLog
            {
                UserId = _currentUser.IsAuthenticated
                    ? _currentUser.UserId.ToString()
                    : null,
                ApiName = apiName,
                RequestJson = JsonSerializer.Serialize(request),
                ResponseJson = response,
                IsSuccess = success,
                RequestDateTime = req,
                ResponseDateTime = responsedt
            };

            _db.SmartPayLogs.Add(log);
            await _db.SaveChangesAsync(cancellationToken);
        }

        // ========= Helper: Safe Deserialize =========
        private T SafeDeserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default!;

            try
            {
                return JsonSerializer.Deserialize<T>(json, _jsonOptions)!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to deserialize SmartPay response: {ex.Message}\nRaw: {json}");
            }
        }

        // ========= 1) Consumer Inquiry =========
        public async Task<SmartPayConsumerInquiryResponse> ConsumerInquiryAsync(
            string cellNo,
            CancellationToken cancellationToken = default)
        {
            const string apiName = "ConsumerInquiry";

            try
            {
                DateTime req= DateTime.Now;
                var url = $"{_options.BaseUrl}/api/ConsumerInq/{cellNo}";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                DateTime responsedt= DateTime.Now;
                
                await LogAsync(req,responsedt,apiName, new { cellNo }, json, response.IsSuccessStatusCode,cancellationToken);

                response.EnsureSuccessStatusCode();
                return SafeDeserialize<SmartPayConsumerInquiryResponse>(json);
            }
            catch (Exception ex)
            {
                await LogAsync(DateTime.Now,DateTime.Now,apiName, new { cellNo }, ex.ToString(), false, cancellationToken);
                throw;
            }
        }

        // ========= 2) Bill Inquiry =========
        public async Task<SmartPayBillInquiryResponse> BillInquiryAsync(
            string consumerNo,
            CancellationToken cancellationToken = default)
        {
            const string apiName = "BillInquiry";

            try
            {
                DateTime req = DateTime.Now;
                var url = $"{_options.BaseUrl}/api/BillInq/{consumerNo}";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                DateTime responsedt = DateTime.Now;

                await LogAsync(req,responsedt,apiName, new { consumerNo }, json, response.IsSuccessStatusCode, cancellationToken);

                response.EnsureSuccessStatusCode();
                return SafeDeserialize<SmartPayBillInquiryResponse>(json);
            }
            catch (Exception ex)
            {
                await LogAsync(DateTime.Now,DateTime.Now,apiName, new { consumerNo }, ex.ToString(), false, cancellationToken);
                throw;
            }
        }

        // ========= 3) Consumer History =========
        public async Task<SmartPayConsumerHistoryResponse> ConsumerHistoryAsync(
            string consumerNo,
            CancellationToken cancellationToken = default)
        {
            const string apiName = "ConsumerHistory";

            try
            {
                DateTime req = DateTime.Now;    
                var url = $"{_options.BaseUrl}/api/ConsumerHistory/{consumerNo}";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                DateTime responsedt = DateTime.Now;

                await LogAsync(req,responsedt,apiName, new { consumerNo }, json, response.IsSuccessStatusCode, cancellationToken);

                response.EnsureSuccessStatusCode();
                return SafeDeserialize<SmartPayConsumerHistoryResponse>(json);
            }
            catch (Exception ex)
            {
                await LogAsync(DateTime.Now,DateTime.Now,apiName, new { consumerNo }, ex.ToString(), false, cancellationToken);
                throw;
            }
        }

        // ========= 4) Upload Bill =========
        public async Task<SmartPayUploadBillResponse> UploadBillAsync(
            SmartPayUploadBillRequest request,
            CancellationToken cancellationToken = default)
        {
            const string apiName = "UploadBill";

            try
            {
                DateTime req = DateTime.Now;
                var url = $"{_options.BaseUrl}/api/AddUpdateBill";    
                var payload = JsonSerializer.Serialize(request);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content, cancellationToken);
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                DateTime responsedt = DateTime.Now;

                await LogAsync(req, responsedt, apiName, request, json, response.IsSuccessStatusCode, cancellationToken);

                response.EnsureSuccessStatusCode();
                return SafeDeserialize<SmartPayUploadBillResponse>(json);
            }
            catch (Exception ex)
            {
                await LogAsync(DateTime.Now,DateTime.Now,apiName, request, ex.ToString(), false, cancellationToken);
                throw;
            }
        }
    }


