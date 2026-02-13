using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class SmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly IApplicationDbContext _context;

    public SmsService(HttpClient httpClient, IConfiguration config, IApplicationDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public async Task<string> SendSmsAsync(string cellnumber, string msg, CancellationToken cancellationToken)
    {
        cellnumber = "923075709068";
        // 1) build your URL
        string url = string.Format("keyhere", cellnumber, msg);

        var response = await _httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<SMSLogVM>(body);
        if (result is null)
            throw new InvalidOperationException("Empty SMS provider response");

   //     await LogToDatabase(result, msg, cancellationToken); // log regardless of success or failure

        if (result.STATUS.Contains("ERROR", StringComparison.OrdinalIgnoreCase))
            return $"{result.STATUS} {result.ERROR_FILTER} {result.ERROR_DESCRIPTION}";

        return result.STATUS; // ✅ success path
    }

   

}
