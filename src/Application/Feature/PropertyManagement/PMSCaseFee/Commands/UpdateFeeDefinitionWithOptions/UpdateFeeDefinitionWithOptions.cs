using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.UpdateFeeDefinitionWithOptions;
public record UpdateFeeDefinitionWithOptionsCommand(
    Guid FeeDefinitionId,

    FeeType FeeType,
    decimal? FixedAmount,
    AreaUnit? AreaUnit,

    bool AllowOverride,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    string? Notes,

    // ✅ For option-based types only (full replace)
    List<FeeOptionInput>? Options
) : IRequest<ApiResult<bool>>;

public class UpdateFeeDefinitionWithOptionsHandler
    : IRequestHandler<UpdateFeeDefinitionWithOptionsCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public UpdateFeeDefinitionWithOptionsHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateFeeDefinitionWithOptionsCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Load FeeDefinition (tracked)
        var feeDef = await _db.Set<FeeDefinition>()
            .FirstOrDefaultAsync(x =>
                x.Id == r.FeeDefinitionId &&
                x.IsDeleted != true,
                ct);

        if (feeDef == null)
            return ApiResult<bool>.Fail("Fee definition not found.");

        // 2️⃣ FeeType validation rules
        bool isOptionBased =
            r.FeeType == FeeType.OptionBased ||
            r.FeeType == FeeType.OptionBasedWithCategory;

        if (!isOptionBased && r.Options != null && r.Options.Any())
            return ApiResult<bool>.Fail("Options are not allowed for this fee type.");

        if (isOptionBased && (r.Options == null || r.Options.Count == 0))
            return ApiResult<bool>.Fail("Options are required for option-based fees.");

        if (isOptionBased && r.FixedAmount != null)
            return ApiResult<bool>.Fail("FixedAmount is not allowed for option-based fees.");

        if (r.FeeType == FeeType.OptionBased &&
            r.Options!.Any(o => o.FeeCategoryId != null))
            return ApiResult<bool>.Fail(
                "FeeCategoryId must be null for OptionBased fees.");

        if (r.FeeType == FeeType.OptionBasedWithCategory &&
            r.Options!.Any(o => o.FeeCategoryId == null))
            return ApiResult<bool>.Fail(
                "FeeCategoryId is required for OptionBasedWithCategory fees.");

        // 3️⃣ Update FeeDefinition fields
        feeDef.FeeType = r.FeeType;
        feeDef.FixedAmount = r.FixedAmount;
        feeDef.AreaUnit = r.AreaUnit;
        feeDef.AllowOverride = r.AllowOverride;
        feeDef.EffectiveFrom = r.EffectiveFrom;
        feeDef.EffectiveTo = r.EffectiveTo;
        feeDef.Notes = r.Notes;

        // 4️⃣ Handle FeeOptions (manual soft delete)
        var existingOptions = await _db.Set<FeeOption>()
            .Where(x =>
                x.FeeDefinitionId == feeDef.Id &&
                x.IsDeleted != true)
            .ToListAsync(ct);

        // Soft delete all existing options
        foreach (var opt in existingOptions)
        {
            opt.IsActive = false;
            opt.IsDeleted = true;
        }

        // Add new options if option-based
        if (isOptionBased)
        {
            var newOptions = r.Options!
                .GroupBy(x =>
                    new { Code = x.Code.Trim().ToUpperInvariant(), x.FeeCategoryId })
                .Select(g => g.First())
                .Select(o => new FeeOption
                {
                    FeeDefinitionId = feeDef.Id,
                    FeeCategoryId = o.FeeCategoryId,
                    Code = o.Code.Trim().ToUpperInvariant(),
                    Name = o.Name.Trim(),
                    ProcessingDays = o.ProcessingDays,
                    Amount = o.Amount,
                    SortOrder = o.SortOrder,
                    IsActive = true,
                    IsDeleted = false
                })
                .ToList();

            _db.Set<FeeOption>().AddRange(newOptions);
        }

        // 5️⃣ Save (identity-safe)
        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(
            true,
            "Fee definition updated successfully.");
    }
}

