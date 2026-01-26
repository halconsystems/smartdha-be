using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.RejectCaseWithRequirements;

public record RejectRequirementInput
(
    Guid PrerequisiteDefinitionId,
    string? Remarks
);
public record RejectCaseWithRequirementsCommand
(
    Guid CaseId,
    string Remarks,
    List<RejectRequirementInput> Requirements
) : IRequest<ApiResult<bool>>;

public class RejectCaseWithRequirementsHandler
    : IRequestHandler<RejectCaseWithRequirementsCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;
    private readonly ICurrentUserService _current;

    public RejectCaseWithRequirementsHandler(
        IPMSApplicationDbContext pmsDb,
        IApplicationDbContext appDb,
        ICurrentUserService current)
    {
        _pmsDb = pmsDb;
        _appDb = appDb;
        _current = current;
    }

    public async Task<ApiResult<bool>> Handle(
        RejectCaseWithRequirementsCommand r,
        CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return ApiResult<bool>.Fail("Unauthorized.");

        if (string.IsNullOrWhiteSpace(r.Remarks))
            return ApiResult<bool>.Fail("Remarks are required.");

        if (r.Requirements == null || r.Requirements.Count == 0)
            return ApiResult<bool>.Fail("At least one required item must be specified.");

        var userId = _current.UserId!.ToString();

        var c = await _pmsDb.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == r.CaseId, ct);

        if (c == null)
            return ApiResult<bool>.Fail("Case not found.");

        if (c.Status == CaseStatus.Approved)
            return ApiResult<bool>.Fail("Case is closed.");

        var step = await _pmsDb.Set<ProcessStep>()
            .Include(x => x.Directorate)
            .FirstAsync(x => x.Id == c.CurrentStepId, ct);

        // Permission check
        if (!await UserInModule(userId, step.Directorate.ModuleId, ct))
            return ApiResult<bool>.Fail("Not allowed.");

        // Ownership rule
        if (!string.IsNullOrEmpty(c.CurrentAssignedUserId) &&
            c.CurrentAssignedUserId != userId)
            return ApiResult<bool>.Fail("Case assigned to another user.");

        // 🔴 Mark case rejected (but re-openable)
        c.Status = CaseStatus.Rejected;
        c.CurrentAssignedUserId = null;

        // 🧱 Save CASE-SPECIFIC missing prerequisites (NO DUPLICATION)
        foreach (var req in r.Requirements)
        {
            // Validate prerequisite belongs to this process
            var valid = await _pmsDb.Set<ProcessPrerequisite>()
                .AnyAsync(x =>
                    x.ProcessId == c.ProcessId &&
                    x.PrerequisiteDefinitionId == req.PrerequisiteDefinitionId,
                    ct);

            if (!valid)
                return ApiResult<bool>.Fail("Invalid prerequisite selected.");

            _pmsDb.Set<CaseRejectRequirement>().Add(new CaseRejectRequirement
            {
                CaseId = c.Id,
                PrerequisiteDefinitionId = req.PrerequisiteDefinitionId,
                Remarks = req.Remarks
            });
        }


        // 🧾 History snapshot
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,

            StepId = step.Id,
            StepNo = step.StepNo,
            StepName = step.Name,

            DirectorateId = step.DirectorateId,
            DirectorateName = step.Directorate.Name,
            ModuleId = step.Directorate.ModuleId,

            Action = CaseAction.Rejected,
            Remarks = r.Remarks,
            FromUserId = userId,
            PerformedByUserId = userId
        });

        await _pmsDb.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Case rejected with required items.");
    }

    private async Task<bool> UserInModule(string userId, Guid moduleId, CancellationToken ct)
    {
        return await _appDb.Set<UserModuleAssignment>()
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.ModuleId == moduleId, ct);
    }
}
