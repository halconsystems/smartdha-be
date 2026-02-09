using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.AddProcessStep.Commands.RemoveProcessStep;

public record RemoveProcessStepCommand(
    Guid ProcessId,
    Guid StepId
) : IRequest<ApiResult<bool>>;

public class RemoveProcessStepHandler
    : IRequestHandler<RemoveProcessStepCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public RemoveProcessStepHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        RemoveProcessStepCommand r,
        CancellationToken ct)
    {
        using var trx = await _db.Database.BeginTransactionAsync(ct);

        // 1️⃣ Load step
        var step = await _db.Set<ProcessStep>()
            .FirstOrDefaultAsync(x =>
                x.Id == r.StepId &&
                x.ProcessId == r.ProcessId &&
                x.IsDeleted != true,
                ct);

        if (step == null)
            return ApiResult<bool>.Fail("Process step not found.");

        // 🚫 Do not allow removing step 1
        if (step.StepNo == 1)
            return ApiResult<bool>.Fail("First step cannot be removed.");

        // 🚫 Block removal if active cases already passed this step
        var hasBlockingCases = await _db.Set<PropertyCase>()
            .AnyAsync(c =>
                c.ProcessId == r.ProcessId &&
                c.IsDeleted != true &&
                c.CurrentStepNo >= step.StepNo,
                ct);

        if (hasBlockingCases)
            return ApiResult<bool>.Fail(
                "Cannot remove this step because active cases have already reached or passed it."
            );

        var removedStepNo = step.StepNo;

        // 2️⃣ Soft delete step7
        step.StepNo = 0; // Temporarily set to 0 to free up the step number for reordering
        step.IsDeleted = true;
        step.IsActive = false;

        // 3️⃣ Reorder remaining steps
        var stepsToReorder = await _db.Set<ProcessStep>()
            .Where(x =>
                x.ProcessId == r.ProcessId &&
                x.IsDeleted != true &&
                x.StepNo > removedStepNo)
            .OrderBy(x => x.StepNo)
            .ToListAsync(ct);

        foreach (var s in stepsToReorder)
        {
            s.StepNo -= 1;
        }

        await _db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return ApiResult<bool>.Ok(true, "Process step removed and reordered successfully.");
    }
}


