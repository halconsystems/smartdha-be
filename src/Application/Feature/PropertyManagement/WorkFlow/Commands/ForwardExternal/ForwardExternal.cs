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

        var fromUserId = _current.UserId!.ToString();

        var c = await _pmsDb.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == r.CaseId && x.IsDeleted !=true, ct);

        if (c == null)
            return ApiResult<bool>.Fail("Case not found.");

        if (c.Status == CaseStatus.Rejected || c.Status == CaseStatus.Approved)
            return ApiResult<bool>.Fail("Case is closed.");

        // 🔹 Load current step with Directorate
        var currentStep = await _pmsDb.Set<ProcessStep>()
            .Include(x => x.Directorate)
            .FirstAsync(x => x.Id == c.CurrentStepId, ct);

        // 🔐 Permission check
        if (!await UserInModule(fromUserId, currentStep.Directorate.ModuleId, ct))
            return ApiResult<bool>.Fail("You are not allowed to act on this directorate.");

        // 🔐 Ownership check
        if (!string.IsNullOrEmpty(c.CurrentAssignedUserId) &&
            c.CurrentAssignedUserId != fromUserId)
            return ApiResult<bool>.Fail("Case is assigned to another user.");

        // 💰 Payment gate
        if (currentStep.RequiresPaymentBeforeNext)
        {
            var hasPaid = await _pmsDb.Set<CasePayment>()
                .AnyAsync(x => x.CaseId == c.Id && x.Status == PaymentStatus.Success, ct);

            if (!hasPaid)
                return ApiResult<bool>.Fail("Payment required before moving forward.");
        }

        // 🔹 Find next step
        var nextStep = await _pmsDb.Set<ProcessStep>()
            .Include(x => x.Directorate)
            .FirstOrDefaultAsync(
                x => x.ProcessId == c.ProcessId &&
                     x.StepNo == currentStep.StepNo + 1,
                ct);

        // ======================================================
        // 🔴 FINAL STEP (no next step)
        // ======================================================
        if (nextStep == null)
        {
            c.Status = CaseStatus.Approved;

            _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
            {
                CaseId = c.Id,

                StepId = currentStep.Id,
                StepNo = currentStep.StepNo,
                StepName = currentStep.Name,

                DirectorateId = currentStep.DirectorateId,
                DirectorateName = currentStep.Directorate.Name,
                ModuleId = currentStep.Directorate.ModuleId,

                Action = CaseAction.ForwardExternal,
                Remarks = r.Remarks,
                FromUserId = fromUserId,
                PerformedByUserId = fromUserId
            });

            await _pmsDb.SaveChangesAsync(ct);
            return ApiResult<bool>.Ok(true, "Case approved (final step).");
        }

        // ======================================================
        // 🟢 NORMAL EXTERNAL FORWARD
        // ======================================================
        var nextmoduleId = await GetDirectorateModuleId(nextStep.DirectorateId, ct);
        // 🔹 Update case runtime state
        c.CurrentStepId = nextStep.Id;
        c.CurrentStepNo = nextStep.StepNo;
        c.CurrentAssignedUserId = null;
        c.Status = CaseStatus.Submitted;
        c.CurrentModuleId = nextStep.Directorate.ModuleId;
        c.DirectorateId = nextStep.DirectorateId;

        // 1️⃣ History: FORWARDED FROM current step
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,

            StepId = currentStep.Id,
            StepNo = currentStep.StepNo,
            StepName = currentStep.Name,

            DirectorateId = currentStep.DirectorateId,
            DirectorateName = currentStep.Directorate.Name,
            ModuleId = currentStep.Directorate.ModuleId,

            Action = CaseAction.ForwardExternal,
            Remarks = r.Remarks,
            FromUserId = fromUserId,
            PerformedByUserId = fromUserId
        });

        // 2️⃣ History: RECEIVED BY next step
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,

            StepId = nextStep.Id,
            StepNo = nextStep.StepNo,
            StepName = nextStep.Name,

            DirectorateId = nextStep.DirectorateId,
            DirectorateName = nextStep.Directorate.Name,
            ModuleId = nextStep.Directorate.ModuleId,

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

