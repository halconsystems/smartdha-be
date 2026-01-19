using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.SubmitCase_V1;
public record SubmitCase_V1Command(
    Guid UserPropertyId,
    Guid ProcessId,
    string? ApplicantName,
    string? ApplicantCnic,
    string? ApplicantMobile,
    string? ApplicantRemarks,
    List<PrerequisiteValueInput> PrerequisiteValues,
    List<IFormFile>? Files
) : IRequest<ApiResult<SubmitCaseResponse>>;

public class SubmitCase_V1Handler: IRequestHandler<SubmitCase_V1Command, ApiResult<SubmitCaseResponse>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public SubmitCase_V1Handler(
        IPMSApplicationDbContext db,
        IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ApiResult<SubmitCaseResponse>> Handle(
        SubmitCase_V1Command r,
        CancellationToken ct)
    {
        using var trx = await _db.Database.BeginTransactionAsync(ct);

        // =========================
        // 1️⃣ CREATE CASE
        // =========================
        var caseNo = $"PMS-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        var c = new PropertyCase
        {
            UserPropertyId = r.UserPropertyId,
            ProcessId = r.ProcessId,
            CaseNo = caseNo,
            Status = CaseStatus.Draft,
            ApplicantName = r.ApplicantName,
            ApplicantCnic = r.ApplicantCnic,
            ApplicantMobile = r.ApplicantMobile,
            ApplicantRemarks = r.ApplicantRemarks,
        };

            _db.Set<PropertyCase>().Add(c);
            await _db.SaveChangesAsync(ct);


        // =========================
        // 2️⃣ SAVE PREREQUISITE VALUES
        // =========================


        //foreach (var p in r.PrerequisiteValues)
        //{
        //    _db.Set<CasePrerequisiteValue>().Add(new CasePrerequisiteValue
        //    {
        //        CaseId = c.Id,
        //        PrerequisiteDefinitionId = p.PrerequisiteDefinitionId,
        //        ValueText = p.ValueText,
        //        ValueNumber = p.ValueNumber,
        //        ValueDate = p.ValueDate,
        //        ValueBool = p.ValueBool
        //    });
        //}

        // 1️⃣ Get all prerequisites attached to this process
        var getAllPre = await _db.Set<ProcessPrerequisite>()
            .Include(x => x.PrerequisiteDefinition)
            .Where(x => x.ProcessId == r.ProcessId)
            .ToListAsync(ct);

        // 2️⃣ Build allowed prerequisite IDs (exclude StaticLabel)
        var allowedPrerequisiteIds = getAllPre
            .Where(x => x.PrerequisiteDefinition.Type != PrerequisiteType.StaticLabel)
            .Select(x => x.PrerequisiteDefinitionId)
            .ToHashSet();

        // 3️⃣ Filter incoming values (only valid + non-static)
        var validValues = r.PrerequisiteValues
            .Where(p => allowedPrerequisiteIds.Contains(p.PrerequisiteDefinitionId))
            .ToList();

        // 4️⃣ Insert values
        foreach (var p in validValues)
        {
            _db.Set<CasePrerequisiteValue>().Add(new CasePrerequisiteValue
            {
                CaseId = c.Id,
                PrerequisiteDefinitionId = p.PrerequisiteDefinitionId,
                ValueText = p.ValueText,
                ValueNumber = p.ValueNumber,
                ValueDate = p.ValueDate,
                ValueBool = p.ValueBool
            });
        }
        await _db.SaveChangesAsync(ct);

        // =========================
        // 3️⃣ SAVE FILES
        // Filename format: {PrerequisiteDefinitionId}__filename.ext
        // =========================
        if (r.Files != null)
        {
            foreach (var file in r.Files)
            {
                Guid? prereqId = null;
                var parts = file.FileName.Split("__", 2);

                if (parts.Length == 2 && Guid.TryParse(parts[0], out var pid))
                    prereqId = pid;

                var path = await _fileStorage.SaveFileAsync(
                    file,
                    $"pms/cases/{c.Id}",
                    ct
                );

                _db.Set<CaseDocument>().Add(new CaseDocument
                {
                    CaseId = c.Id,
                    PrerequisiteDefinitionId = prereqId,
                    FileName = file.FileName,
                    FileUrl = path,
                    ContentType = file.ContentType,
                    FileSize = file.Length
                });
            }
        }

        await _db.SaveChangesAsync(ct);

        // =========================
        // 4️⃣ VALIDATION
        // =========================

        var processPrereqs = await _db.Set<ProcessPrerequisite>()
            .Include(x => x.PrerequisiteDefinition)
            .Where(x =>
                x.ProcessId == r.ProcessId &&
                x.RequiredByStepNo == 0 &&
                x.PrerequisiteDefinition.Type != PrerequisiteType.StaticLabel)
            .ToListAsync(ct);

        var providedValueIds = await _db.Set<CasePrerequisiteValue>()
            .Where(x => x.CaseId == c.Id)
            .Select(x => x.PrerequisiteDefinitionId)
            .ToListAsync(ct);

        var providedFileIds = await _db.Set<CaseDocument>()
            .Where(x => x.CaseId == c.Id && x.PrerequisiteDefinitionId != null)
            .Select(x => x.PrerequisiteDefinitionId!.Value)
            .ToListAsync(ct);

        var providedAll = providedValueIds
            .Union(providedFileIds)
            .Distinct()
            .ToList();

        // ❌ Missing required
        var missing = processPrereqs
            .Where(x => x.IsRequired && !providedAll.Contains(x.PrerequisiteDefinitionId))
            .ToList();

        if (missing.Any())
            return ApiResult<SubmitCaseResponse>.Fail(
                $"Missing required prerequisites: {missing.Count}");

        // =========================
        // 4️⃣.1 OPTION VALIDATION
        // =========================
        foreach (var value in r.PrerequisiteValues)
        {
            var def = await _db.Set<PrerequisiteDefinition>()
                .FirstAsync(x => x.Id == value.PrerequisiteDefinitionId, ct);

            if (!IsOptionBased(def.Type))
                continue;

            var allowed = await _db.Set<PrerequisiteOption>()
                .Where(x => x.PrerequisiteDefinitionId == def.Id && !x.IsDisabled)
                .Select(x => x.Value)
                .ToListAsync(ct);

            if (def.Type is PrerequisiteType.Dropdown or PrerequisiteType.RadioGroup)
            {
                if (string.IsNullOrWhiteSpace(value.ValueText) ||
                    !allowed.Contains(value.ValueText))
                    return ApiResult<SubmitCaseResponse>.Fail(
                        $"Invalid value for {def.Name}");
            }

            if (def.Type is PrerequisiteType.MultiSelect or PrerequisiteType.CheckboxGroup)
            {
                var selected = JsonSerializer.Deserialize<List<string>>(value.ValueText ?? "[]");
                if (selected == null || selected.Any(x => !allowed.Contains(x)))
                    return ApiResult<SubmitCaseResponse>.Fail(
                        $"Invalid selection for {def.Name}");
            }
        }

        // =========================
        // 5️⃣ MOVE TO STEP-1
        // =========================
        var step1 = await _db.Set<ProcessStep>()
            .FirstAsync(x => x.ProcessId == r.ProcessId && x.StepNo == 1, ct);

        c.Status = CaseStatus.Submitted;
        c.CurrentStepNo = 1;
        c.CurrentStepId = step1.Id;

        _db.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,
            StepId = step1.Id,
            Action = StepAction.Submit,
            Remarks = "Submitted via generic form",
            NextStepId = step1.Id,
            NextStepNo = 1
        });

        await _db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return ApiResult<SubmitCaseResponse>.Ok(
            new SubmitCaseResponse(c.Id, caseNo),
            "Case submitted successfully");
    }

    private static bool IsOptionBased(PrerequisiteType type) =>
        type == PrerequisiteType.Dropdown ||
        type == PrerequisiteType.MultiSelect ||
        type == PrerequisiteType.CheckboxGroup ||
        type == PrerequisiteType.RadioGroup;
}
