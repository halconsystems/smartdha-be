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

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.ResubmitRejectedCase;
public class ResubmitRejectedCaseRequest
{
    public string PrerequisiteValuesJson { get; set; } = default!;
    public List<IFormFile>? Files { get; set; }
}
public record ResubmitRejectedCaseCommand(
    Guid CaseId,
    List<PrerequisiteValueInput> PrerequisiteValues,
    List<IFormFile>? Files
) : IRequest<ApiResult<bool>>;

public class ResubmitRejectedCaseHandler
    : IRequestHandler<ResubmitRejectedCaseCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _current;

    public ResubmitRejectedCaseHandler(
        IPMSApplicationDbContext db,
        IFileStorageService fileStorage,
        ICurrentUserService current)
    {
        _db = db;
        _fileStorage = fileStorage;
        _current = current;
    }

    public async Task<ApiResult<bool>> Handle(
        ResubmitRejectedCaseCommand r,
        CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return ApiResult<bool>.Fail("Unauthorized.");

        var userId = _current.UserId!.ToString();

        using var trx = await _db.Database.BeginTransactionAsync(ct);

        // 1️⃣ Load case
        var c = await _db.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == r.CaseId, ct);

        if (c == null)
            return ApiResult<bool>.Fail("Case not found.");

        if (c.Status != CaseStatus.Rejected)
            return ApiResult<bool>.Fail("Case is not in rejected state.");

        // 2️⃣ Load rejected requirements
        var rejectedReqs = await _db.Set<CaseRejectRequirement>()
            .Where(x => x.CaseId == c.Id)
            .ToListAsync(ct);

        if (!rejectedReqs.Any())
            return ApiResult<bool>.Fail("No rejected requirements found.");

        var rejectedReqMap = rejectedReqs
            .ToDictionary(x => x.PrerequisiteDefinitionId, x => x);

        // =========================
        // 3️⃣ Save prerequisite VALUES (non-file)
        // =========================
        foreach (var value in r.PrerequisiteValues)
        {
            if (!rejectedReqMap.ContainsKey(value.PrerequisiteDefinitionId))
                continue;

            _db.Set<CasePrerequisiteValue>().Add(new CasePrerequisiteValue
            {
                CaseId = c.Id,
                PrerequisiteDefinitionId = value.PrerequisiteDefinitionId,
                ValueText = value.ValueText,
                ValueNumber = value.ValueNumber,
                ValueDate = value.ValueDate,
                ValueBool = value.ValueBool,
                CaseRejectRequirementId =
                    rejectedReqMap[value.PrerequisiteDefinitionId].Id
            });
        }

        await _db.SaveChangesAsync(ct);

        // =========================
        // 4️⃣ Save FILE uploads (rejected only)
        // filename format: {PrerequisiteDefinitionId}__filename.ext
        // =========================
        if (r.Files != null)
        {
            foreach (var file in r.Files)
            {
                Guid prereqDefId;
                var parts = file.FileName.Split("__", 2);

                if (parts.Length != 2 ||
                    !Guid.TryParse(parts[0], out prereqDefId) ||
                    !rejectedReqMap.ContainsKey(prereqDefId))
                {
                    continue; // ignore unrelated file
                }

                var rejectReq = rejectedReqMap[prereqDefId];

                var path = await _fileStorage.SavePMSDocumentAsync(
                    file,
                    $"pms/cases/{c.Id}/rejected",
                    ct,
                    maxBytes: 10 * 1024 * 1024,
                    allowedExtensions: null
                );

                _db.Set<CaseDocument>().Add(new CaseDocument
                {
                    CaseId = c.Id,
                    PrerequisiteDefinitionId = prereqDefId,
                    CaseRejectRequirementId = rejectReq.Id, // 🔴 KEY
                    FileName = file.FileName,
                    FileUrl = path,
                    ContentType = file.ContentType,
                    FileSize = file.Length
                });

                // mark uploaded
                rejectReq.IsUploaded = true;
            }
        }

        await _db.SaveChangesAsync(ct);

        // =========================
        // 5️⃣ Validate all rejected items uploaded
        // =========================
        if (rejectedReqs.Any(x => !x.IsUploaded))
            return ApiResult<bool>.Fail("Please upload all required documents.");

        // =========================
        // 6️⃣ Clear rejection & resubmit
        // =========================
        c.Status = CaseStatus.Submitted;
        c.CurrentAssignedUserId = null;

        _db.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,
            StepId = c.CurrentStepId!.Value,
            StepNo = c.CurrentStepNo,
            Action = CaseAction.Resubmitted,
            Remarks = "Applicant resubmitted missing documents.",
            PerformedByUserId = userId
        });

        await _db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return ApiResult<bool>.Ok(true, "Case resubmitted successfully.");
    }
}


