using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.UpdatePrerequisiteDefinition;

public record UpdatePrerequisiteDefinitionCommand(
    Guid Id,
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions,
    List<PrerequisiteOptionInput>? Options
) : IRequest<ApiResult<bool>>;


public class UpdatePrerequisiteDefinitionHandler
    : IRequestHandler<UpdatePrerequisiteDefinitionCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public UpdatePrerequisiteDefinitionHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdatePrerequisiteDefinitionCommand r,
        CancellationToken ct)
    {
        var entity = await _db.Set<PrerequisiteDefinition>()
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Prerequisite not found.");

        var code = r.Code.Trim().ToUpperInvariant();

        // 1️⃣ Code uniqueness check
        var codeExists = await _db.Set<PrerequisiteDefinition>()
            .AnyAsync(x => x.Code == code && x.Id != r.Id, ct);

        if (codeExists)
            return ApiResult<bool>.Fail("Prerequisite code already exists.");

        // 2️⃣ Update base definition
        entity.Name = r.Name.Trim();
        entity.Code = code;
        entity.Type = r.Type;
        entity.MinLength = r.MinLength;
        entity.MaxLength = r.MaxLength;
        entity.AllowedExtensions = r.AllowedExtensions;

        // 3️⃣ Option-based handling
        bool needsOptions =
            r.Type == PrerequisiteType.Dropdown ||
            r.Type == PrerequisiteType.MultiSelect ||
            r.Type == PrerequisiteType.CheckboxGroup ||
            r.Type == PrerequisiteType.RadioGroup;

        var existingOptions = await _db.Set<PrerequisiteOption>()
            .Where(x => x.PrerequisiteDefinitionId == r.Id)
            .ToListAsync(ct);

        if (needsOptions)
        {
            if (r.Options == null || r.Options.Count == 0)
                return ApiResult<bool>.Fail(
                    "Options are required for dropdown / multiselect / checkbox / radio types."
                );

            var incoming = r.Options
                .GroupBy(x => x.Value.Trim())
                .Select(g => g.First())
                .ToList();

            // 🔹 DELETE removed options
            var toRemove = existingOptions
                .Where(e => !incoming.Any(i => i.Value.Trim() == e.Value))
                .ToList();

            if (toRemove.Count > 0)
                _db.Set<PrerequisiteOption>().RemoveRange(toRemove);

            // 🔹 UPDATE existing options
            foreach (var opt in existingOptions)
            {
                var match = incoming.FirstOrDefault(x => x.Value.Trim() == opt.Value);
                if (match != null)
                {
                    opt.Label = match.Label.Trim();
                    opt.SortOrder = match.SortOrder;
                }
            }

            // 🔹 ADD new options
            var toAdd = incoming
                .Where(i => !existingOptions.Any(e => e.Value == i.Value.Trim()))
                .Select(i => new PrerequisiteOption
                {
                    PrerequisiteDefinitionId = r.Id,
                    Label = i.Label.Trim(),
                    Value = i.Value.Trim(),
                    SortOrder = i.SortOrder
                })
                .ToList();

            if (toAdd.Count > 0)
                _db.Set<PrerequisiteOption>().AddRange(toAdd);
        }
        else
        {
            // 🔹 If type changed to non-option, remove all options
            if (existingOptions.Count > 0)
                _db.Set<PrerequisiteOption>().RemoveRange(existingOptions);
        }

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Prerequisite updated.");
    }
}
