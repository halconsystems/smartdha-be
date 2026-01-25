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

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.WorkFlow.Commands.ForwardExternal;
public record ForwardExternalCommand(
    Guid CaseId,
    string? Remarks
) : IRequest<ApiResult<bool>>;
public class ForwardExternalHandler : IRequestHandler<ForwardExternalCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;
    private readonly ICurrentUserService _current;

    public ForwardExternalHandler(IPMSApplicationDbContext pmsDb, IApplicationDbContext appDb, ICurrentUserService current)
    {
        _pmsDb = pmsDb;
        _appDb = appDb;
        _current = current;
    }

    public async Task<ApiResult<bool>> Handle(ForwardExternalCommand r, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return ApiResult<bool>.Fail("Unauthorized.");

        var fromUserId = _current.UserId.ToString()!;

        var c = await _pmsDb.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == r.CaseId && x.IsDeleted !=true, ct);

        if (c == null) return ApiResult<bool>.Fail("Case not found.");
        if (c.Status == CaseStatus.Rejected || c.Status == CaseStatus.Approved)
            return ApiResult<bool>.Fail("Case is closed.");

        var currentStep = await _pmsDb.Set<ProcessStep>()
            .FirstAsync(x => x.Id == c.CurrentStepId, ct);

        // Permission: user must be in current directorate module
        var moduleId = await GetDirectorateModuleId(currentStep.DirectorateId, ct);
        if (!await UserInModule(fromUserId, moduleId, ct))
            return ApiResult<bool>.Fail("You are not allowed to act on this directorate.");

        // Ownership rule: only current assigned user can forward externally (or add admin bypass)
        if (!string.IsNullOrEmpty(c.CurrentAssignedUserId) && c.CurrentAssignedUserId != fromUserId)
            return ApiResult<bool>.Fail("Case is assigned to another user.");

        // Payment gate if required
        if (currentStep.RequiresPaymentBeforeNext)
        {
            var hasPaid = await _pmsDb.Set<CasePayment>()
                .AnyAsync(x => x.CaseId == c.Id && x.Status == PaymentStatus.Success, ct);

            if (!hasPaid)
                return ApiResult<bool>.Fail("Payment required before moving forward.");
        }

        // Find next external step
        var nextStep = await _pmsDb.Set<ProcessStep>()
            .FirstOrDefaultAsync(x => x.ProcessId == c.ProcessId && x.StepNo == currentStep.StepNo + 1, ct);

        if (nextStep == null)
        {
            // No next step -> final approval
            c.Status = CaseStatus.Approved;

            _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
            {
                CaseId = c.Id,
                StepId = currentStep.Id,
                FromUserId = fromUserId,
                ToUserId = null,
                Action = CaseAction.ForwardExternal,
                Remarks = r.Remarks,
            });

            await _pmsDb.SaveChangesAsync(ct);
            return ApiResult<bool>.Ok(true, "Case approved (final step).");
        }

        // Move to next directorate step
        c.CurrentStepId = nextStep.Id;
        c.CurrentStepNo = nextStep.StepNo;
        c.CurrentAssignedUserId = null;   // reset internal assignment
        c.Status = CaseStatus.InProgress;
        c.CurrentModuleId=nextStep.Directorate.ModuleId;
        c.DirectorateId=nextStep.DirectorateId;

        // Add history of external forward
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,
            StepId = currentStep.Id,
            FromUserId = fromUserId,
            ToUserId = null,
            Action = CaseAction.ForwardExternal,
            Remarks = r.Remarks
        });

        // Also optionally add "Received" record for next step (recommended)
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,
            StepId = nextStep.Id,
            FromUserId = null,
            ToUserId = null,
            Action = CaseAction.Received,
            Remarks = "Received in next directorate."
        });

        await _pmsDb.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Forwarded to next directorate.");
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

