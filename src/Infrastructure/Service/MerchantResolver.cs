using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class MerchantResolver : IMerchantResolver
{
    private readonly PaymentDbContext _db;

    public MerchantResolver(PaymentDbContext db)
    {
        _db = db;
    }

    public async Task<string> ResolveAsync(
        string sourceSystem,
        string entityType,
        Guid? entityId,
        CancellationToken ct)
    {
        // 1️⃣ Try MOST SPECIFIC rule first
        var rule = await _db.PayMerchantRules
            .AsNoTracking()
            .Where(x =>
                x.IsActive == true &&
                x.SourceSystem == sourceSystem &&
                x.EntityType == entityType &&
                x.EntityId == entityId)
            .OrderByDescending(x => x.Priority)
            .FirstOrDefaultAsync(ct);

        if (rule != null)
            return rule.MerchantCode;

        // 2️⃣ Fallback rule (EntityId = NULL)
        rule = await _db.PayMerchantRules
            .AsNoTracking()
            .Where(x =>
                x.IsActive==true &&
                x.SourceSystem == sourceSystem &&
                x.EntityType == entityType &&
                x.EntityId == null)
            .OrderByDescending(x => x.Priority)
            .FirstOrDefaultAsync(ct);

        if (rule != null)
            return rule.MerchantCode;

        // 3️⃣ Absolute fallback (single default merchant)
        var defaultMerchant = await _db.PayMerchants
            .AsNoTracking()
            .Where(x => x.IsActive == true)
            .OrderBy(x => x.CreatedAt)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(ct);

        if (defaultMerchant == null)
            throw new InvalidOperationException(
                $"No active merchant configured for {sourceSystem}/{entityType}");

        return defaultMerchant;
    }

    public async Task<string> ResolveBySmartPayCodeAsync(
    string smartPayMerchantId,
    CancellationToken ct)
    {
        var merchant = await _db.PayMerchants
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.IsActive == true &&
                x.SmartPayMerchantId == smartPayMerchantId,
                ct);

        if (merchant == null)
            throw new InvalidOperationException(
                $"No merchant found for SmartPayMerchantId {smartPayMerchantId}");

        return merchant.Code;
    }


}

