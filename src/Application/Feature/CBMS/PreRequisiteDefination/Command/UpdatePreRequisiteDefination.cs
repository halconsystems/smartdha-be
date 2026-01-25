using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisiteDefination.Command;

public record UpdateClubPrerequisiteDefinitionCommand(
    Guid Id,
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions,
    // NEW (only for Dropdown / MultiSelect / Checkbox / Radio)
    List<PrerequisiteOptionInput>? Options
) : IRequest<ApiResult<Guid>>;

public class UpdateClubPrerequisiteDefinitionCommandHandler
    : IRequestHandler<UpdateClubPrerequisiteDefinitionCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public UpdateClubPrerequisiteDefinitionCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        UpdateClubPrerequisiteDefinitionCommand r,
        CancellationToken ct)
    {
        var entity = await _db.Set<ClubPrerequisiteDefinitions>()
            .FirstOrDefaultAsync(x => x.Id == r.Id);

        if (entity == null)
            return ApiResult<Guid>.Fail("Prerequisite Not Found");

        var code = r.Code.Trim().ToUpperInvariant();

        // 1️⃣ Check duplicate definition
        var exists = await _db.Set<ClubPrerequisiteDefinitions>()
            .AnyAsync(x => x.Code == code, ct);

        if (exists)
            return ApiResult<Guid>.Fail("Prerequisite code already exists.");

        entity.Name = r.Name.Trim();
        entity.Code = code;
        entity.Type = r.Type;
        entity.MinLength = r.MinLength;
        entity.MaxLength = r.MaxLength;
        entity.AllowedExtensions = r.AllowedExtensions;

        await _db.SaveChangesAsync(ct); // ensure def.Id


        var preOptions = await _db.Set<ClubPrerequisiteOptions>()
            .Where(x => x.PrerequisiteDefinitionId == r.Id)
            .ToListAsync(ct);

        if (preOptions != null)
        {
            bool needsOptions =
            r.Type == PrerequisiteType.Dropdown ||
            r.Type == PrerequisiteType.MultiSelect ||
            r.Type == PrerequisiteType.CheckboxGroup ||
            r.Type == PrerequisiteType.RadioGroup;

            if (needsOptions)
            {
                if (r.Options == null || r.Options.Count == 0)
                    return ApiResult<Guid>.Fail(
                        "Options are required for dropdown / multiselect / checkbox / radio types."
                    );

                var incomingOptions = r.Options
                    .GroupBy(x => x.Value.Trim())
                    .Select(g => g.First())
                    .ToList();

                foreach (var opt in incomingOptions)
                {
                    var existing = preOptions
                        .FirstOrDefault(x => x.Value == opt.Value);

                    if (existing != null)
                    {
                        // UPDATE
                        existing.Label = opt.Label.Trim();
                        existing.Value = opt.Value.Trim();
                        existing.SortOrder = opt.SortOrder;
                    }
                    else
                    {
                        // ADD NEW
                        _db.Set<ClubPrerequisiteOptions>().Add(new ClubPrerequisiteOptions
                        {
                            PrerequisiteDefinitionId = r.Id,
                            Label = opt.Label.Trim(),
                            Value = opt.Value.Trim(),
                            SortOrder = opt.SortOrder
                        });
                    }
                }
                await _db.SaveChangesAsync(ct);
            }
        }

        // 3️⃣ Handle option-based types


        return ApiResult<Guid>.Ok(entity.Id, "Prerequisite definition created.");
    }
}




