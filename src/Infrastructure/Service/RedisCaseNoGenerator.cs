using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class DbCaseNoGenerator : ICaseNoGenerator
{
    private readonly IApplicationDbContext _ctx;
    public DbCaseNoGenerator(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<string> NextAsync(CancellationToken ct = default)
    {
        var start = DateTime.UtcNow.Date;         // 00:00 UTC today
        var end = start.AddDays(1);             // 00:00 UTC tomorrow

        // Get the highest CaseNo for *today* (prefix is same, suffix is 6-digit zero-padded, so string order works)
        var last = await _ctx.PanicRequests.AsNoTracking()
            .Where(x => x.Created >= start && x.Created < end)
            .OrderByDescending(x => x.CaseNo)
            .Select(x => x.CaseNo)
            .FirstOrDefaultAsync(ct);

        var nextSeq = 1;
        if (!string.IsNullOrEmpty(last))
        {
            // PAN-YYYYMMDD-######  -> take last segment
            var parts = last.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out var n))
                nextSeq = n + 1;
        }

        var dateStr = start.ToString("yyyyMMdd");
        return $"PAN-{dateStr}-{nextSeq:000000}";
    }
}
