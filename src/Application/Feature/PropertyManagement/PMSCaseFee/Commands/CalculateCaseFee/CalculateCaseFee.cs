using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.CalculateCaseFee;

using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using MediatR;

public record CalculateCaseFeeCommand(
    Guid CaseId,
    decimal? PropertyArea,
    AreaUnit? AreaUnit
) : IRequest<ApiResult<Guid>>;

public class CalculateCaseFeeHandler
    : IRequestHandler<CalculateCaseFeeCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;

    public CalculateCaseFeeHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CalculateCaseFeeCommand request,
        CancellationToken ct)
    {
        var c = await _db.Set<PropertyCase>()
            .Include(x => x.Process)
            .FirstOrDefaultAsync(x => x.Id == request.CaseId, ct);

        if (c == null)
            return ApiResult<Guid>.Fail("Case not found.");

        var feeDef = await _db.Set<FeeDefinition>()
            .FirstOrDefaultAsync(x => x.ProcessId == c.ProcessId, ct);

        if (feeDef == null)
            return ApiResult<Guid>.Fail("Fee not configured for this process.");

        decimal amount = 0;
        Guid? slabId = null;

        if (feeDef.FeeType == FeeType.Fixed)
        {
            amount = feeDef.FixedAmount ?? 0;
        }
        else if (feeDef.FeeType == FeeType.AreaBased)
        {
            if (!request.PropertyArea.HasValue)
                return ApiResult<Guid>.Fail("Property area is required.");

            var slab = await _db.Set<FeeSlab>()
                .Where(x =>
                    x.FeeDefinitionId == feeDef.Id &&
                    request.PropertyArea >= x.FromArea &&
                    request.PropertyArea <= x.ToArea)
                .FirstOrDefaultAsync(ct);

            if (slab == null)
                return ApiResult<Guid>.Fail("No fee slab found for given area.");

            amount = slab.Amount;
            slabId = slab.Id;
        }
        else if (feeDef.FeeType == FeeType.Manual)
        {
            amount = 0;
        }

        var existing = await _db.Set<CaseFee>()
            .FirstOrDefaultAsync(x => x.CaseId == request.CaseId, ct);

        if (existing != null)
            return ApiResult<Guid>.Fail("Case fee already calculated.");

        var caseFee = new CaseFee
        {
            CaseId = request.CaseId,
            FeeDefinitionId = feeDef.Id,
            PropertyArea = request.PropertyArea,
            AreaUnit = request.AreaUnit,
            Amount = amount,
            FeeSlabId = slabId
        };

        _db.Set<CaseFee>().Add(caseFee);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(caseFee.Id, "Case fee calculated.");
    }
}

