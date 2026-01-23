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

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.WorkFlow.Commands.ForwardInternal;
public record ForwardInternalCommand(
    Guid CaseId,
    string ToUserId,
    string? Remarks
) : IRequest<ApiResult<bool>>;
public class ForwardInternalHandler : IRequestHandler<ForwardInternalCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;
    private readonly ICurrentUserService _current;

    public ForwardInternalHandler(IPMSApplicationDbContext pmsDb, IApplicationDbContext appDb, ICurrentUserService current)
    {
        _pmsDb = pmsDb;
        _appDb = appDb;
        _current = current;
    }

    public async Task<ApiResult<bool>> Handle(ForwardInternalCommand r, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return ApiResult<bool>.Fail("Unauthorized.");

        if (string.IsNullOrWhiteSpace(r.ToUserId))
            return ApiResult<bool>.Fail("ToUserId is required.");

        var caseEntity = await _pmsDb.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == r.CaseId, ct);

        if (caseEntity == null)
            return ApiResult<bool>.Fail("Case not found.");

        // terminal checks
        if (caseEntity.Status == CaseStatus.Rejected || caseEntity.Status == CaseStatus.Approved)
            return ApiResult<bool>.Fail("Case is closed.");

        // current external step
        var step = await _pmsDb.Set<ProcessStep>()
            .AsNoTracking()
            .FirstAsync(x => x.Id == caseEntity.CurrentStepId, ct);

        // get module for this directorate
        var moduleId = await GetDirectorateModuleId(step.DirectorateId, ct);

        // Ensure current user belongs to module (can see this directorate)
        var fromUserId = _current.UserId.ToString()!;
        if (!await UserInModule(fromUserId, moduleId, ct))
            return ApiResult<bool>.Fail("You are not allowed to act on this directorate.");

        // Ensure target user belongs to SAME module
        if (!await UserInModule(r.ToUserId, moduleId, ct))
            return ApiResult<bool>.Fail("Target user does not belong to this directorate/module.");

        // Ownership rule (important):
        // - if case is unassigned => allow directorate users to assign
        // - if assigned => only assigned user can forward (or add 'directorate admin' bypass later)
        if (!string.IsNullOrEmpty(caseEntity.CurrentAssignedUserId) &&
            caseEntity.CurrentAssignedUserId != fromUserId)
        {
            return ApiResult<bool>.Fail("Case is assigned to another user.");
        }

        // Update assignment (runtime)
        caseEntity.CurrentAssignedUserId = r.ToUserId;
        caseEntity.Status = CaseStatus.InProgress;

        // History (immutable)
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = caseEntity.Id,
            StepId = step.Id,
            FromUserId = fromUserId,
            ToUserId = r.ToUserId,
            Action = CaseAction.ForwardInternal,
            Remarks = r.Remarks
        });

        await _pmsDb.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Forwarded internally.");
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

