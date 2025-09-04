using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Infrastructure.Service;

public class ActivityLogger : IActivityLogger
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActivityLogger(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(
        string action,
        string? userId = null,
        string? email = null,
        string? cnic = null,
        string? description = null,
        AppType appType = AppType.Web,
        CancellationToken ct = default)
    {
        var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

        var log = new UserActivityLog
        {
            UserId = userId,
            Email = email,
            CNIC = cnic,
            Action = action,
            Description = description,
            IpAddress = ip,
            Device = userAgent,
            Browser = userAgent,
            Timestamp = DateTime.Now,
            AppType = appType
        };
        try
        {
            _context.UserActivityLogs.Add(log);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}


