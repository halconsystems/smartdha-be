using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSFeeSetting.Queries.GetFeeSettings;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetFeeDefinitionByCaseId;
public record GetFeeDefinitionByCaseIdQuery(Guid CaseId)
    : IRequest<ApiResult<FeeDefinitionDto>>;
public class GetFeeDefinitionByCaseIdHandler
    : IRequestHandler<GetFeeDefinitionByCaseIdQuery, ApiResult<FeeDefinitionDto>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetFeeDefinitionByCaseIdHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<FeeDefinitionDto>> Handle(
        GetFeeDefinitionByCaseIdQuery r,
        CancellationToken ct)
    {
        // 1️⃣ Load case + process
        var c = await _db.Set<PropertyCase>()
            .Include(x => x.Process)
            .FirstOrDefaultAsync(x => x.Id == r.CaseId && x.IsDeleted != true, ct);

        if (c == null)
            return ApiResult<FeeDefinitionDto>.Fail("Case not found.");

        var process = c.Process;
        if (process == null)
            return ApiResult<FeeDefinitionDto>.Fail("Process not found for case.");

        // 2️⃣ Fee Definition
        var feeDef = await _db.Set<FeeDefinition>()
            .FirstOrDefaultAsync(x =>
                x.ProcessId == process.Id &&
                x.IsDeleted == false,
                ct);

        if (feeDef == null)
            return ApiResult<FeeDefinitionDto>.Fail("Fee definition not found.");

        // 3️⃣ Categories
        var categories = await _db.Set<FeeCategory>()
            .OrderBy(x => x.Code)
            .ToListAsync(ct);

        // 4️⃣ Options
        var options = await _db.Set<FeeOption>()
            .Where(x =>
                x.FeeDefinitionId == feeDef.Id &&
                x.IsDeleted == false)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(ct);

        // 5️⃣ Extra charges (FeeSettings)
        var feeSettingQuery = _db.Set<FeeSetting>()
            .Where(x => x.IsDeleted != true && x.IsActive == true);

        if (!process.IsNadraVerificationRequired)
            feeSettingQuery = feeSettingQuery.Where(x => x.Code != "NADRA_FEE");

        var feeSettings = await feeSettingQuery
            .OrderBy(x => x.DisplayName)
            .Select(x => new FeeSettingDto(
                x.Id,
                x.Code,
                x.DisplayName,
                x.Amount
            ))
            .ToListAsync(ct);

        // ======================================================
        // OPTION BASED WITH CATEGORY
        // ======================================================
        if (feeDef.FeeType == FeeType.OptionBasedWithCategory)
        {
            var categoryDtos = categories
                .Select(cat =>
                {
                    var catOptions = options
                        .Where(o => o.FeeCategoryId == cat.Id)
                        .Select(o => new FeeOptionDto(
                            o.Id,
                            o.Code,
                            o.Name,
                            o.ProcessingDays,
                            o.Amount,
                            o.SortOrder
                        ))
                        .ToList();

                    return new FeeCategoryDto(
                        cat.Id,
                        cat.Code,
                        cat.Name,
                        catOptions
                    );
                })
                .Where(c => c.Options.Any())
                .ToList();

            return ApiResult<FeeDefinitionDto>.Ok(new FeeDefinitionDto(
                feeDef.Id,
                feeDef.FeeType,
                feeDef.FixedAmount,
                feeDef.AreaUnit,
                feeDef.AllowOverride,
                feeDef.EffectiveFrom,
                feeDef.EffectiveTo,
                feeDef.Notes,
                categoryDtos,
                null,
                feeSettings
            ));
        }

        // ======================================================
        // OPTION BASED (NO CATEGORY)
        // ======================================================
        if (feeDef.FeeType == FeeType.OptionBased)
        {
            var optionDtos = options
                .Where(o => o.FeeCategoryId == null)
                .Select(o => new FeeOptionDto(
                    o.Id,
                    o.Code,
                    o.Name,
                    o.ProcessingDays,
                    o.Amount,
                    o.SortOrder
                ))
                .ToList();

            return ApiResult<FeeDefinitionDto>.Ok(new FeeDefinitionDto(
                feeDef.Id,
                feeDef.FeeType,
                feeDef.FixedAmount,
                feeDef.AreaUnit,
                feeDef.AllowOverride,
                feeDef.EffectiveFrom,
                feeDef.EffectiveTo,
                feeDef.Notes,
                null,
                optionDtos,
                feeSettings
            ));
        }

        // ======================================================
        // FIXED / AREA / MANUAL
        // ======================================================
        return ApiResult<FeeDefinitionDto>.Ok(new FeeDefinitionDto(
            feeDef.Id,
            feeDef.FeeType,
            feeDef.FixedAmount,
            feeDef.AreaUnit,
            feeDef.AllowOverride,
            feeDef.EffectiveFrom,
            feeDef.EffectiveTo,
            feeDef.Notes,
            null,
            null,
            feeSettings
        ));
    }
}
