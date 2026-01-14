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
    public MoveCaseHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<bool>> Handle(MoveCaseCommand r, CancellationToken ct)
    {
        var c = await _db.Set<PropertyCase>().FirstOrDefaultAsync(x => x.Id == r.CaseId, ct);
        if (c == null) return ApiResult<bool>.Fail("Case not found.");
        if (c.CurrentStepNo <= 0) return ApiResult<bool>.Fail("Case is not in workflow.");

        var currentStep = await _db.Set<ProcessStep>().FirstAsync(x => x.Id == c.CurrentStepId, ct);

        // Action routing (simple)
        int? nextStepNo = null;

        if (r.Action == StepAction.Forward || r.Action == StepAction.Approve)
            nextStepNo = c.CurrentStepNo + 1;

        if (r.Action == StepAction.Return)
            nextStepNo = Math.Max(1, c.CurrentStepNo - 1);

        if (r.Action == StepAction.Reject)
        {
            c.Status = CaseStatus.Rejected;
            nextStepNo = null;
        }

        // If moving forward, ensure next step exists or finish case
        Guid? nextStepId = null;

        if (nextStepNo.HasValue)
        {
            var next = await _db.Set<ProcessStep>()
                .FirstOrDefaultAsync(x => x.ProcessId == c.ProcessId && x.StepNo == nextStepNo.Value, ct);

            if (next == null)
            {
                // No next step = final approval
                c.Status = CaseStatus.Approved;
                c.CurrentStepNo = c.CurrentStepNo; // keep last
                c.CurrentStepId = c.CurrentStepId;
            }
            else
            {
                // Payment gating (if current step requires payment before next)
                if (currentStep.RequiresPaymentBeforeNext)
                {
                    var hasPaid = await _db.Set<CasePayment>()
                        .AnyAsync(x => x.CaseId == c.Id && x.Status == PaymentStatus.Success, ct);

                    if (!hasPaid) return ApiResult<bool>.Fail("Payment required before moving forward.");
                }

                c.Status = CaseStatus.InProgress;
                c.CurrentStepNo = next.StepNo;
                c.CurrentStepId = next.Id;
                nextStepId = next.Id;
            }
        }

        _db.Set<CaseStepHistory>().Add(new CaseStepHistory
        {
            CaseId = c.Id,
            StepId = currentStep.Id,
            Action = r.Action,
            Remarks = r.Remarks,
            NextStepId = nextStepId,
            NextStepNo = nextStepNo
        });

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Case updated.");
    }
}

