using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands;

public class SubmitCaseRequest
{
    // Case
    public Guid UserPropertyId { get; set; }
    public Guid ProcessId { get; set; }

    public string? ApplicantName { get; set; }
    public string? ApplicantCnic { get; set; }
    public string? ApplicantMobile { get; set; }
    public string? ApplicantRemarks { get; set; }

    // Prerequisite VALUES (JSON string)
    // Example: [{ "prerequisiteDefinitionId":"...", "valueText":"ABC Solar" }]
    public string PrerequisiteValuesJson { get; set; } = default!;

    // Documents (key = prerequisiteDefinitionId)
    public List<IFormFile>? Files { get; set; }
}


public class PrerequisiteValueInput
{
    public Guid PrerequisiteDefinitionId { get; set; }
    public string? ValueText { get; set; }
    public decimal? ValueNumber { get; set; }
    public DateTime? ValueDate { get; set; }
    public bool? ValueBool { get; set; }
}



public record SubmitCaseCommand(
    Guid UserPropertyId,
    Guid ProcessId,
    string? ApplicantName,
    string? ApplicantCnic,
    string? ApplicantMobile,
    string? ApplicantRemarks,
    List<PrerequisiteValueInput> PrerequisiteValues,
    List<IFormFile>? Files
) : IRequest<ApiResult<SubmitCaseResponse>>;

public record SubmitCaseResponse(Guid CaseId, string CaseNo);

public class SubmitCaseHandler
    : IRequestHandler<SubmitCaseCommand, ApiResult<SubmitCaseResponse>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public SubmitCaseHandler(
        IPMSApplicationDbContext db,
        IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ApiResult<SubmitCaseResponse>> Handle(
        SubmitCaseCommand r,
        CancellationToken ct)
    {
        using var trx = await _db.Database.BeginTransactionAsync(ct);

        // 1️⃣ Create Case
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
            ApplicantRemarks = r.ApplicantRemarks
        };

        _db.Set<PropertyCase>().Add(c);
        await _db.SaveChangesAsync(ct);

        // 2️⃣ Save prerequisite values
        foreach (var p in r.PrerequisiteValues)
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

        // 3️⃣ Save files
        if (r.Files != null)
        {
            foreach (var file in r.Files)
            {
                // Extract prerequisiteDefinitionId from filename
                // Format: {guid}__filename.ext
                var parts = file.FileName.Split("__", 2);
                Guid? prereqId = null;

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

        // 4️⃣ Validate required prerequisites (submission)
        var requiredDefs = await _db.Set<ProcessPrerequisite>()
            .Where(x => x.ProcessId == r.ProcessId && x.IsRequired && x.RequiredByStepNo == 0)
            .Select(x => x.PrerequisiteDefinitionId)
            .ToListAsync(ct);

        

        // Get provided NON-FILE prerequisites
        var providedValueDefs = await _db.Set<CasePrerequisiteValue>()
            .Where(x => x.CaseId == c.Id)
            .Select(x => x.PrerequisiteDefinitionId)
            .ToListAsync(ct);

        // Get provided FILE prerequisites
        var providedFileDefs = await _db.Set<CaseDocument>()
            .Where(x => x.CaseId == c.Id && x.PrerequisiteDefinitionId != null)
            .Select(x => x.PrerequisiteDefinitionId!.Value)
            .ToListAsync(ct);

        // Merge both
        var providedAllDefs = providedValueDefs
            .Union(providedFileDefs)
            .Distinct()
            .ToList();

        // Validate
        var missing = requiredDefs.Except(providedAllDefs).ToList();

        if (missing.Any())
        {
            return ApiResult<SubmitCaseResponse>.Fail(
                $"Missing required prerequisites: {missing.Count}");
        }


        // 5️⃣ Move to Step-1
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
            Remarks = "Submitted in single request",
            NextStepId = step1.Id,
            NextStepNo = 1
        });

        await _db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return ApiResult<SubmitCaseResponse>.Ok(
            new SubmitCaseResponse(c.Id, caseNo),
            "Case submitted successfully");
    }
}


