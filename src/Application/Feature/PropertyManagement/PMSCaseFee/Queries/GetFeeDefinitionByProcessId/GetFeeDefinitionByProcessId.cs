using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeDefinitionByProcessId;
public record GetFeeDefinitionByProcessIdQuery(Guid ProcessId) : IRequest<ApiResult<FeeDefinitionDto>>;
public class GetFeeDefinitionByProcessIdHandler : IRequestHandler<GetFeeDefinitionByProcessIdQuery, ApiResult<FeeDefinitionDto>>
{
    private readonly IPMSApplicationDbContext _db;
    public GetFeeDefinitionByProcessIdHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<ApiResult<FeeDefinitionDto>> Handle(
        GetFeeDefinitionByProcessIdQuery r,
        CancellationToken ct)
    {
        var feeDef = await _db.Set<FeeDefinition>()
            .FirstOrDefaultAsync(x => x.ProcessId == r.ProcessId, ct);

        if (feeDef == null)
            return ApiResult<FeeDefinitionDto>.Fail("Fee definition not found.");

        var categories = await _db.Set<FeeCategory>()
    .OrderBy(x => x.Code)
    .ToListAsync(ct);

        var options = await _db.Set<FeeOption>()
    .Where(o => o.FeeDefinitionId == feeDef.Id)
    .OrderBy(o => o.SortOrder)
    .ToListAsync(ct);

        // OPTION BASED WITH CATEGORY
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
                o.Amount
            ))
            .ToList();

        return new FeeCategoryDto(
            cat.Id,
            cat.Code,
            cat.Name,
            catOptions
        );
    })
    .Where(c => c.Options.Any()) // ✅ NOW SAFE
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
                null
            ));
        }

        // OPTION BASED (NO CATEGORY)
        if (feeDef.FeeType == FeeType.OptionBased)
        {
            var finaloptions = await _db.Set<FeeOption>()
                .Where(x =>
                    x.FeeDefinitionId == feeDef.Id &&
                    x.FeeCategoryId == null)
                .OrderBy(x => x.SortOrder)
                .Select(o => new FeeOptionDto(
                    o.Id,
                    o.Code,
                    o.Name,
                    o.ProcessingDays,
                    o.Amount
                ))
                .ToListAsync(ct);

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
                finaloptions
            ));
        }

        // FIXED / AREA / MANUAL
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
            null
        ));
    }
}

