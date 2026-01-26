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
public record ReturnCaseCommand(
    Guid CaseId,
    string Remarks
) : IRequest<ApiResult<bool>>;
public class ReturnCaseHandler
    : IRequestHandler<ReturnCaseCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;
    private readonly ICurrentUserService _current;

    public ReturnCaseHandler(
        IPMSApplicationDbContext pmsDb,
        IApplicationDbContext appDb,
        ICurrentUserService current)
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
            return ApiResult<bool>.Fail("Remarks required.");

        var userId = _current.UserId!.ToString();

        var c = await _pmsDb.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == r.CaseId && x.IsDeleted !=true, ct);

        if (c == null)
            return ApiResult<bool>.Fail("Case not found.");

        if (c.Status == CaseStatus.Approved || c.Status == CaseStatus.Rejected)
            return ApiResult<bool>.Fail("Case is closed.");

        var currentStep = await _pmsDb.Set<ProcessStep>()
            .Include(x => x.Directorate)
            .FirstAsync(x => x.Id == c.CurrentStepId, ct);

        // Permission
        if (!await UserInModule(userId, currentStep.Directorate.ModuleId, ct))
            return ApiResult<bool>.Fail("Not allowed.");

        // Find previous step
        var prevStep = await _pmsDb.Set<ProcessStep>()
            .Include(x => x.Directorate)
            .FirstOrDefaultAsync(x =>
                x.ProcessId == c.ProcessId &&
                x.StepNo == currentStep.StepNo - 1,
                ct);

        if (prevStep == null)
            return ApiResult<bool>.Fail("No previous step found.");

        // Update case location
        c.CurrentStepId = prevStep.Id;
        c.CurrentStepNo = prevStep.StepNo;
        c.CurrentAssignedUserId = null;
        c.Status = CaseStatus.Returned;
        c.CurrentModuleId = prevStep.Directorate.ModuleId;
        c.DirectorateId = prevStep.DirectorateId;

        // History: returned from current step
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,

            StepId = currentStep.Id,
            StepNo = currentStep.StepNo,
            StepName = currentStep.Name,
            DirectorateId = currentStep.DirectorateId,
            DirectorateName = currentStep.Directorate.Name,
            ModuleId = currentStep.Directorate.ModuleId,

            Action = CaseAction.Returned,
            Remarks = r.Remarks,
            FromUserId = userId,
            PerformedByUserId = userId
        });

        // History: received by previous step
        _pmsDb.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,

            StepId = prevStep.Id,
            StepNo = prevStep.StepNo,
            StepName = prevStep.Name,
            DirectorateId = prevStep.DirectorateId,
            DirectorateName = prevStep.Directorate.Name,
            ModuleId = prevStep.Directorate.ModuleId,

            Action = CaseAction.Received,
            Remarks = "Returned from next directorate."
        });

        await _pmsDb.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Case returned to previous directorate.");
    }

    private async Task<bool> UserInModule(string userId, Guid moduleId, CancellationToken ct)
    {
        return await _appDb.Set<UserModuleAssignment>()
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.ModuleId == moduleId, ct);
    }
}


