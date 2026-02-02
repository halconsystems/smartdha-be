using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class SmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly IApplicationDbContext _context;

    public SmsService(HttpClient httpClient, IConfiguration config,IApplicationDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public async Task<string> SendSmsAsync(string cellnumber, string msg,CancellationToken cancellationToken)
    {
        cellnumber = "923075709068";
        // 1) build your URL
        string url = string.Format("https://api.veevotech.com/v3/sendsms?hash=4186056c9b55fa16d3006446d1a5d7c0&receivernum=+{0}&textmessage={1}", cellnumber, msg);

        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<SMSLogVM>(body);
        if (result is null)
            throw new InvalidOperationException("Empty SMS provider response");

        await LogToDatabase(result, msg,cancellationToken); // log regardless of success or failure

        if (result.STATUS.Contains("ERROR", StringComparison.OrdinalIgnoreCase))
            return $"{result.STATUS} {result.ERROR_FILTER} {result.ERROR_DESCRIPTION}";

        return result.STATUS; // ✅ success path
    }

    private async Task LogToDatabase(SMSLogVM result, string sentMessage, CancellationToken cancellationToken)
    {
        var isSuccess = !result.STATUS.Contains("ERROR", StringComparison.OrdinalIgnoreCase);

        var entity = new SMSLog
        {
            Status = result.STATUS,
            MessageId = result.MESSAGE_ID,
            CountrySupported = result.COUNTRY_SUPPORTED,
            CountryCode = result.COUNTRY_CODE,
            CountryIso = result.COUNTRY_ISO,
            NetworkName = result.NETWORK_NAME,
            ReceiverNumber = result.RECEIVER_NUMBER,
            ErrorFilter = result.ERROR_FILTER,
            ErrorCode = result.ERROR_CODE,
            ErrorDescription = result.ERROR_DESCRIPTION ?? (isSuccess ? "SMS sent successfully." : null),
            ChargedBalance = result.CHARGED_BALANCE,
            ChargingUnit = result.CHARGING_UNIT,
            SentMessage = sentMessage,
            IsSmsSent = isSuccess
        };

        _context.SMSLogs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

}
