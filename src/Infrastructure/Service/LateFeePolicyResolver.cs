using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
using DHAFacilitationAPIs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class LateFeePolicyResolver : ILateFeePolicyResolver
{
    private readonly PaymentDbContext _db;

    public LateFeePolicyResolver(PaymentDbContext db)
    {
        _db = db;
    }

    public async Task<PayLateFeePolicy> ResolveAsync(
        string sourceSystem,
        CancellationToken ct)
    {
        var policy = await _db.Set<PayLateFeePolicy>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.SourceSystem == sourceSystem &&
                x.IsActive==true,
                ct);

        if (policy == null)
            throw new InvalidOperationException(
                $"Late fee policy not configured for {sourceSystem}");

        return policy;
    }
}

