using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands;
public record MoveCaseCommand(
    Guid CaseId,
    StepAction Action,
    string? Remarks
) : IRequest<ApiResult<bool>>;

public class MoveCaseHandler : IRequestHandler<MoveCaseCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly ICurrentUserService _currentUserService;
    public MoveCaseHandler(IPMSApplicationDbContext db, ICurrentUserService _currentUserService)
    {
        _db = db;
        this._currentUserService = _currentUserService;
    }

    public async Task<ApiResult<bool>> Handle(MoveCaseCommand r, CancellationToken ct)
    {
        var userId = _currentUserService.UserId.ToString() ?? throw new UnauthorizedAccessException("Invalid user.");

        var c = await _db.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == r.CaseId, ct);

        if (c == null)
            return ApiResult<bool>.Fail("Case not found.");

        var currentStep = await _db.Set<ProcessStep>()
            .FirstAsync(x => x.Id == c.CurrentStepId, ct);

        Guid? nextProcessStepId = null;
        int? nextInternalStepNo = null;

        switch (r.Action)
        {
            // 🔁 INTERNAL MOVE
            case StepAction.Complete:
                {
                    c.CurrentInternalStepNo ??= 1;
                    nextInternalStepNo = c.CurrentInternalStepNo + 1;

                    c.CurrentInternalStepNo = nextInternalStepNo;
                    c.Status = CaseStatus.InProgress;
                    break;
                }

            // ➡️ EXTERNAL FORWARD
            case StepAction.Forward:
                {
                    // payment gate
                    if (currentStep.RequiresPaymentBeforeNext)
                    {
                        var hasPaid = await _db.Set<CasePayment>()
                            .AnyAsync(x => x.CaseId == c.Id && x.Status == PaymentStatus.Success, ct);

                        if (!hasPaid)
                            return ApiResult<bool>.Fail("Payment required before moving forward.");
                    }

                    var nextStep = await _db.Set<ProcessStep>()
                        .FirstOrDefaultAsync(
                            x => x.ProcessId == c.ProcessId &&
                                 x.StepNo == currentStep.StepNo + 1,
                            ct);

                    if (nextStep == null)
                    {
                        c.Status = CaseStatus.Approved;
                        break;
                    }

                    c.CurrentStepId = nextStep.Id;
                    c.CurrentStepNo = nextStep.StepNo;
                    c.CurrentInternalStepNo = null; // 🔑 reset
                    c.Status = CaseStatus.InProgress;

                    nextProcessStepId = nextStep.Id;
                    break;
                }

            // ❌ REJECT
            case StepAction.Reject:
                {
                    c.Status = CaseStatus.Rejected;
                    break;
                }

            // 🔙 RETURN (external backward – optional)
            case StepAction.Return:
                {
                    var prevStep = await _db.Set<ProcessStep>()
                        .FirstOrDefaultAsync(
                            x => x.ProcessId == c.ProcessId &&
                                 x.StepNo == currentStep.StepNo - 1,
                            ct);

                    if (prevStep == null)
                        return ApiResult<bool>.Fail("No previous step.");

                    c.CurrentStepId = prevStep.Id;
                    c.CurrentStepNo = prevStep.StepNo;
                    c.Status = CaseStatus.InProgress;

                    nextProcessStepId = prevStep.Id;
                    break;
                }
        }

        // ✅ HISTORY (IMMUTABLE)
        _db.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,
            StepId = currentStep.Id,
            Action = CaseAction.ForwardExternal,
            Remarks = r.Remarks,
            PerformedByUserId=userId
        });

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Case updated.");
    }

}

