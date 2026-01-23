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

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.WorkFlow.Commands.ReturnCase;
public record ReturnCaseCommand(Guid CaseId, string Remarks)
    : IRequest<ApiResult<bool>>;
public class ReturnCaseHandler : IRequestHandler<ReturnCaseCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;
    private readonly ICurrentUserService _current;

    public ReturnCaseHandler(IPMSApplicationDbContext pmsDb, IApplicationDbContext appDb, ICurrentUserService current)
    {
        _pmsDb = pmsDb;
        _appDb = appDb;
        _current = current;
    }

    public async Task<ApiResult<bool>> Handle(ReturnCaseCommand r, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return ApiResult<bool>.Fail("Unauthorized.");

        if (string.IsNullOrWhiteSpace(r.Remarks))
            return ApiResult<bool>.Fail("Remarks are required.");

        var userId = _current.UserId.ToString()!;

        var c = await _pmsDb.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == r.CaseId, ct);

        if (c == null) return ApiResult<bool>.Fail("Case not found.");
        if (c.Status == CaseStatus.Rejected || c.Status == CaseStatus.Approved)
            return ApiResult<bool>.Fail("Case is closed.");

        var currentStep = await _pmsDb.Set<ProcessStep>().FirstAsync(x => x.Id == c.CurrentStepId, ct);

        // permission
        var moduleId = await GetDirectorateModuleId(currentStep.DirectorateId, ct);
        if (!await UserInModule(userId, moduleId, ct))
            return ApiResult<bool>.Fail("You are not allowed to act on this directorate.");

        // ownership optional
        if (!string.IsNullOrEmpty(c.CurrentAssignedUserId) && c.CurrentAssignedUserId != userId)
            return ApiResult<bool>.Fail("Case is assigned to another user.");

        // find previous step (linear)
        var prevStep = await _pmsDb.Set<ProcessStep>()
            .FirstOrDefaultAsync(x => x.ProcessId == c.ProcessId && x.StepNo == currentStep.StepNo - 1, ct);

        if (prevStep == null)
            return ApiResult<bool>.Fail("No previous step found.");

        // move back externally
        c.CurrentStepId = prevStep.Id;
        c.CurrentStepNo = prevStep.StepNo;
        c.CurrentAssignedUserId = null;
        c.Status = CaseStatus.Returned;

        // history
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,
            StepId = currentStep.Id,
            FromUserId = userId,
            Action = CaseAction.Returned,
            Remarks = r.Remarks
        });

        // optionally: received at previous directorate
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,
            StepId = prevStep.Id,
            Action = CaseAction.Received,
            Remarks = "Returned to previous directorate."
        });

        await _pmsDb.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Case returned to previous step.");
    }

    private async Task<Guid> GetDirectorateModuleId(Guid directorateId, CancellationToken ct)
    {
        var dir = await _pmsDb.Set<Directorate>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == directorateId, ct);

        if (dir == null) throw new InvalidOperationException("Directorate not found.");
        return dir.ModuleId;
    }

    private async Task<bool> UserInModule(string userId, Guid moduleId, CancellationToken ct)
    {
        return await _appDb.Set<UserModuleAssignment>()
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.ModuleId == moduleId, ct);
    }
}

