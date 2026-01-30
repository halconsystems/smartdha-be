using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSPrerequisiteDefinition.Commands;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.PreRequisiteDefination.Command;

public record CreateClubPrerequisiteDefinitionCommand(

     Guid ProcessId,
    string Name,
    string Code,
    PrerequisiteType Type,
    int? MinLength,
    int? MaxLength,
    string? AllowedExtensions,
    bool IsRequired,
    int RequiredByStepNo,
    // NEW (only for Dropdown / MultiSelect / Checkbox / Radio)
    List<PrerequisiteOptionInput>? Options
) : IRequest<ApiResult<Guid>>;

public class CreateClubPrerequisiteDefinitionCommandHandler
    : IRequestHandler<CreateClubPrerequisiteDefinitionCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public CreateClubPrerequisiteDefinitionCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(
        CreateClubPrerequisiteDefinitionCommand r,
        CancellationToken ct)
    {

        var process = await _db.Set<ClubServiceProcess>()
           .FirstOrDefaultAsync(x => x.Id == r.ProcessId, ct);

        if (process == null)
            return ApiResult<Guid>.Fail("Process not found.");

        var code = r.Code.Trim().ToUpperInvariant();

        // 1️⃣ Check duplicate definition
        var exists = await _db.Set<ClubPrerequisiteDefinitions>()
            .AnyAsync(x => x.Code == code, ct);

        if (exists)
            return ApiResult<Guid>.Fail("Prerequisite code already exists.");

        // 2️⃣ Create definition
        var def = new ClubPrerequisiteDefinitions
        {
            Name = r.Name.Trim(),
            Code = code,
            Type = r.Type,
            MinLength = r.MinLength,
            MaxLength = r.MaxLength,
            AllowedExtensions = r.AllowedExtensions
        };

        _db.Set<ClubPrerequisiteDefinitions>().Add(def);
        await _db.SaveChangesAsync(ct); // ensure def.Id


        // 3️⃣ Check duplicate attachment
        var alreadyAttached = await _db.Set<ClubProcessPrerequisite>()
            .AnyAsync(x =>
                x.ProcessId == r.ProcessId &&
                x.PrerequisiteDefinitionId == def.Id,
                ct);

        if (alreadyAttached)
            return ApiResult<Guid>.Fail("Prerequisite already attached to this process.");


        // 4️⃣ Attach to process
        var mapping = new ClubProcessPrerequisite
        {
            ProcessId = r.ProcessId,
            PrerequisiteDefinitionId = def.Id,
            IsRequired = r.IsRequired,
            RequiredByStepNo = r.RequiredByStepNo
        };

        _db.Set<ClubProcessPrerequisite>().Add(mapping);

        bool isStaticLabel = r.Type == PrerequisiteType.StaticLabel;

        // ❌ StaticLabel must not have options
        if (isStaticLabel && r.Options != null && r.Options.Count > 0)
        {
            return ApiResult<Guid>.Fail("StaticLabel cannot have options.");
        }

        // 3️⃣ Handle option-based types
        bool needsOptions =
            r.Type == PrerequisiteType.Dropdown ||
            r.Type == PrerequisiteType.MultiSelect ||
            r.Type == PrerequisiteType.CheckboxGroup ||
            r.Type == PrerequisiteType.RadioGroup;

        //if (needsOptions)
        //{
        //    if (r.Options == null || r.Options.Count == 0)
        //        return ApiResult<Guid>.Fail(
        //            "Options are required for dropdown / multiselect / checkbox / radio types."
        //        );

        //    var options = r.Options
        //        .GroupBy(x => x.Value.Trim())
        //        .Select(g => g.First())
        //        .Select(x => new ClubPrerequisiteOptions
        //        {
        //            PrerequisiteDefinitionId = def.Id,
        //            Label = x.Label.Trim(),
        //            Value = x.Value.Trim(),
        //            SortOrder = x.SortOrder
        //        })
        //        .ToList();

        //    _db.Set<ClubPrerequisiteOptions>().AddRange(options);
        //    await _db.SaveChangesAsync(ct);
        //}

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
            var existingValues = await _db.Set<ClubPrerequisiteOptions>()
                .Where(x => x.PrerequisiteDefinitionId == def.Id)
                .Select(x => x.Value)
                .ToListAsync(ct);

            var newOptions = distinct
                .Where(x => !existingValues.Contains(x.Value.Trim()))
                .Select(x => new ClubPrerequisiteOptions
                {
                    PrerequisiteDefinitionId = def.Id,
                    Label = x.Label.Trim(),
                    Value = x.Value.Trim(),
                    SortOrder = x.SortOrder
                })
                .ToList();

            if (newOptions.Count > 0)
            {
                _db.Set<ClubPrerequisiteOptions>().AddRange(newOptions);

            }
        }
        await _db.SaveChangesAsync(ct);
        return ApiResult<Guid>.Ok(def.Id, "Prerequisite definition created.");
    }
}



