using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FeeDefinationWithOption.Command;

public record CreateClubFeeDefinitionWithOptionsCommand(
    Guid ProcessId,
    FeeType FeeType,

    // Fixed / AreaBased
    decimal? FixedAmount,
    AreaUnit? AreaUnit,

    bool AllowOverride,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    string? Notes,

    // ONLY used for OptionBased / OptionBasedWithCategory
    List<FeeOptionInput>? Options
) : IRequest<ApiResult<Guid>>;
public class CreateClubFeeDefinitionWithOptionsCommandHandler
    : IRequestHandler<CreateClubFeeDefinitionWithOptionsCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public CreateClubFeeDefinitionWithOptionsCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateClubFeeDefinitionWithOptionsCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Ensure one FeeDefinition per process
        var exists = await _db.Set<ClubFeeDefinition>()
            .FirstOrDefaultAsync(x => x.ProcessId == r.ProcessId && x.IsActive == true && x.IsDeleted == false, ct);

        if (exists != null)
            return ApiResult<Guid>.Fail("Fee definition already exists for this process.");

        // 2️⃣ Validate FeeType rules
        bool isOptionBased =
            r.FeeType == FeeType.OptionBased ||
            r.FeeType == FeeType.OptionBasedWithCategory;

        if (!isOptionBased && r.Options != null && r.Options.Any())
            return ApiResult<Guid>.Fail("Options are not allowed for this fee type.");

        if (isOptionBased && (r.Options == null || r.Options.Count == 0))
            return ApiResult<Guid>.Fail("Options are required for option-based fees.");

        if (isOptionBased && r.FixedAmount != null)
            return ApiResult<Guid>.Fail("FixedAmount is not allowed for option-based fees.");

        // 3️⃣ Create FeeDefinition
        var feeDef = new ClubFeeDefinition
        {
            ProcessId = r.ProcessId,
            FeeType = r.FeeType,
            FixedAmount = r.FixedAmount,
            AreaUnit = r.AreaUnit,
            AllowOverride = r.AllowOverride,
            EffectiveFrom = r.EffectiveFrom,
            EffectiveTo = r.EffectiveTo,
            Notes = r.Notes
        };

        _db.Set<ClubFeeDefinition>().Add(feeDef);
        await _db.SaveChangesAsync(ct); // ensure Id

        // 4️⃣ Create FeeOptions (if applicable)
        if (isOptionBased)
        {
            var options = r.Options!
                .GroupBy(x => new { x.Code, x.FeeCategoryId }) // avoid duplicates
                .Select(g => g.First())
                .Select(o => new Domain.Entities.CBMS.ClubFeeOption
                {
                    FeeDefinitionId = feeDef.Id,
                    FeeCategoryId = o.FeeCategoryId,
                    Code = o.Code.Trim().ToUpperInvariant(),
                    Name = o.Name.Trim(),
                    ProcessingDays = o.ProcessingDays,
                    Amount = o.Amount,
                    SortOrder = o.SortOrder
                })
                .ToList();

            _db.Set<Domain.Entities.CBMS.ClubFeeOption>().AddRange(options);
            await _db.SaveChangesAsync(ct);
        }

        return ApiResult<Guid>.Ok(
            feeDef.Id,
            "Fee definition created successfully."
        );
    }
}

