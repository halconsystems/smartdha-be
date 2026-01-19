using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreatePrerequisiteDefinition;

public record CreatePrerequisiteDefinitionCommand(
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions,
    // NEW (only for Dropdown / MultiSelect / Checkbox / Radio)
    List<PrerequisiteOptionInput>? Options
) : IRequest<ApiResult<Guid>>;

public class CreatePrerequisiteDefinitionHandler
    : IRequestHandler<CreatePrerequisiteDefinitionCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;

    public CreatePrerequisiteDefinitionHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreatePrerequisiteDefinitionCommand r,
        CancellationToken ct)
    {
        var code = r.Code.Trim().ToUpperInvariant();

        // 1️⃣ Check duplicate definition
        var exists = await _db.Set<PrerequisiteDefinition>()
            .AnyAsync(x => x.Code == code, ct);

        if (exists)
            return ApiResult<Guid>.Fail("Prerequisite code already exists.");

        // 2️⃣ Create definition
        var def = new PrerequisiteDefinition
        {
            Name = r.Name.Trim(),
            Code = code,
            Type = r.Type,
            MinLength = r.MinLength,
            MaxLength = r.MaxLength,
            AllowedExtensions = r.AllowedExtensions
        };

        _db.Set<PrerequisiteDefinition>().Add(def);
        await _db.SaveChangesAsync(ct); // ensure def.Id

        // 3️⃣ Handle option-based types
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

            var options = r.Options
                .GroupBy(x => x.Value.Trim())
                .Select(g => g.First())
                .Select(x => new PrerequisiteOption
                {
                    PrerequisiteDefinitionId = def.Id,
                    Label = x.Label.Trim(),
                    Value = x.Value.Trim(),
                    SortOrder = x.SortOrder
                })
                .ToList();

            _db.Set<PrerequisiteOption>().AddRange(options);
            await _db.SaveChangesAsync(ct);
        }

        return ApiResult<Guid>.Ok(def.Id, "Prerequisite definition created.");
    }
}


