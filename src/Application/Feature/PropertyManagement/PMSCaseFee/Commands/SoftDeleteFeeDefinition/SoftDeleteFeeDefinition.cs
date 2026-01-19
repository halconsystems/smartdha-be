using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.SoftDeleteFeeDefinition;
public record SoftDeleteFeeDefinitionCommand(
    Guid FeeDefinitionId
) : IRequest<ApiResult<bool>>;

public class SoftDeleteFeeDefinitionHandler
    : IRequestHandler<SoftDeleteFeeDefinitionCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public SoftDeleteFeeDefinitionHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        SoftDeleteFeeDefinitionCommand r,
        CancellationToken ct)
    {
        var feeDef = await _db.Set<FeeDefinition>()
            .FirstOrDefaultAsync(x => x.Id == r.FeeDefinitionId && x.IsDeleted != true, ct);

        if (feeDef == null)
            return ApiResult<bool>.Fail("Fee definition not found.");

        // Soft delete definition
        feeDef.IsActive = false;
        feeDef.IsDeleted = true;

        // Soft delete options
        var options = await _db.Set<FeeOption>()
            .Where(x => x.FeeDefinitionId == feeDef.Id && x.IsDeleted != true)
            .ToListAsync(ct);

        foreach (var opt in options)
        {
            opt.IsActive = false;
            opt.IsDeleted = true;
        }

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Fee definition deleted.");
    }
}

