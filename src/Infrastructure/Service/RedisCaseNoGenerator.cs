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
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // Get most recent CaseNo for today
        var last = await _ctx.PanicRequests.AsNoTracking()
            .Where(x => x.Created >= today && x.Created < tomorrow)
            .OrderByDescending(x => x.CaseNo)
            .Select(x => x.CaseNo)
            .FirstOrDefaultAsync(ct);

        int nextSeq = 1;

        if (!string.IsNullOrEmpty(last))
        {
            // Expected format: P-ddMMyy-###
            var parts = last.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int seq))
                nextSeq = seq + 1;
        }

        string date = today.ToString("ddMMyy");

        return $"P-{date}-{nextSeq:000}";
    }

}
