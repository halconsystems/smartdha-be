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

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.WorkFlow.Commands.RejectCase;
public record RejectCaseCommand(Guid CaseId, string Remarks)
    : IRequest<ApiResult<bool>>;
public class RejectCaseHandler : IRequestHandler<RejectCaseCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;
    private readonly ICurrentUserService _current;

    public RejectCaseHandler(IPMSApplicationDbContext pmsDb, IApplicationDbContext appDb, ICurrentUserService current)
    {
        _pmsDb = pmsDb;
        _appDb = appDb;
        _current = current;
    }

    public async Task<ApiResult<bool>> Handle(RejectCaseCommand r, CancellationToken ct)
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

        var step = await _pmsDb.Set<ProcessStep>().FirstAsync(x => x.Id == c.CurrentStepId, ct);

        var moduleId = await GetDirectorateModuleId(step.DirectorateId, ct);
        if (!await UserInModule(userId, moduleId, ct))
            return ApiResult<bool>.Fail("You are not allowed to act on this directorate.");

        // optional ownership restriction
        if (!string.IsNullOrEmpty(c.CurrentAssignedUserId) && c.CurrentAssignedUserId != userId)
            return ApiResult<bool>.Fail("Case is assigned to another user.");

        c.Status = CaseStatus.Rejected;

        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,
            StepId = step.Id,
            FromUserId = userId,
            ToUserId = null,
            Action = CaseAction.Rejected,
            Remarks = r.Remarks
        });

        await _pmsDb.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Case rejected.");
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

