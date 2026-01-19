using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands.CreateAndAttachPrerequisite;

public record CreateAndAttachPrerequisiteCommand
(
    Guid ProcessId,
    // Definition data
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions,
    // Process mapping
    bool IsRequired,
    int RequiredByStepNo,
    //NEW: For Dropdown/MultiSelect/CheckboxGroup/RadioGroup
    List<PrerequisiteOptionInput>? Options
) : IRequest<ApiResult<Guid>>;


public class CreateAndAttachPrerequisiteHandler
    : IRequestHandler<CreateAndAttachPrerequisiteCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;

    public CreateAndAttachPrerequisiteHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateAndAttachPrerequisiteCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Validate process
        var process = await _db.Set<ServiceProcess>()
            .FirstOrDefaultAsync(x => x.Id == r.ProcessId, ct);

        if (process == null)
            return ApiResult<Guid>.Fail("Process not found.");

        // 2️⃣ Get or create PrerequisiteDefinition
        var definition = await _db.Set<PrerequisiteDefinition>()
            .FirstOrDefaultAsync(x => x.Code == r.Code, ct);

        if (definition == null)
        {
            definition = new PrerequisiteDefinition
            {
                Name = r.Name.Trim(),
                Code = r.Code.Trim().ToUpperInvariant(),
                Type = r.Type,
                MinLength = r.MinLength,
                MaxLength = r.MaxLength,
                AllowedExtensions = r.AllowedExtensions
            };

            _db.Set<PrerequisiteDefinition>().Add(definition);
            await _db.SaveChangesAsync(ct); // ensure Id generated
        }

        // 3️⃣ Check duplicate attachment
        var alreadyAttached = await _db.Set<ProcessPrerequisite>()
            .AnyAsync(x =>
                x.ProcessId == r.ProcessId &&
                x.PrerequisiteDefinitionId == definition.Id,
                ct);

        if (alreadyAttached)
            return ApiResult<Guid>.Fail("Prerequisite already attached to this process.");

        // 4️⃣ Attach to process
        var mapping = new ProcessPrerequisite
        {
            ProcessId = r.ProcessId,
            PrerequisiteDefinitionId = definition.Id,
            IsRequired = r.IsRequired,
            RequiredByStepNo = r.RequiredByStepNo
        };

        _db.Set<ProcessPrerequisite>().Add(mapping);

        bool isStaticLabel = r.Type == PrerequisiteType.StaticLabel;

        // ❌ StaticLabel must not have options
        if (isStaticLabel && r.Options != null && r.Options.Count > 0)
        {
            return ApiResult<Guid>.Fail("StaticLabel cannot have options.");
        }

        // ✅ Allowed types that need options
        bool needsOptions =
            r.Type == PrerequisiteType.Dropdown ||
            r.Type == PrerequisiteType.MultiSelect ||
            r.Type == PrerequisiteType.CheckboxGroup ||
            r.Type == PrerequisiteType.RadioGroup;

        if (needsOptions)
        {
            if (r.Options == null || r.Options.Count == 0)
                return ApiResult<Guid>.Fail("Options are required for dropdown/multiselect/checkbox/radio types.");

            // Remove duplicates in input
            var distinct = r.Options
                .GroupBy(x => x.Value.Trim())
                .Select(g => g.First())
                .ToList();

            // Insert options (only those not already present)
            var existingValues = await _db.Set<PrerequisiteOption>()
                .Where(x => x.PrerequisiteDefinitionId == definition.Id)
                .Select(x => x.Value)
                .ToListAsync(ct);

            var newOptions = distinct
                .Where(x => !existingValues.Contains(x.Value.Trim()))
                .Select(x => new PrerequisiteOption
                {
                    PrerequisiteDefinitionId = definition.Id,
                    Label = x.Label.Trim(),
                    Value = x.Value.Trim(),
                    SortOrder = x.SortOrder
                })
                .ToList();

            if (newOptions.Count > 0)
            {
                _db.Set<PrerequisiteOption>().AddRange(newOptions);
                
            }
        }

        await _db.SaveChangesAsync(ct);
        return ApiResult<Guid>.Ok(mapping.Id, "Prerequisite created and attached.");
    }
}

