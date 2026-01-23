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
    Guid ProcessId
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

        // 1️⃣ Validate step
        var step = await _db.Set<ProcessStep>()
            .FirstOrDefaultAsync(x =>
                x.ProcessId == r.ProcessId &&
                x.IsDeleted != true,
                ct);

        if (step == null)
            return ApiResult<bool>.Fail("Process step not found.");

        var removedStepNo = step.StepNo;

        // 2️⃣ Soft delete (recommended)
        step.IsDeleted = true;
        step.IsActive = false;

        // 3️⃣ Reorder remaining steps
        var stepsToUpdate = await _db.Set<ProcessStep>()
            .Where(x =>
                x.ProcessId == r.ProcessId &&
                x.IsDeleted != true &&
                x.StepNo > removedStepNo)
            .OrderBy(x => x.StepNo)
            .ToListAsync(ct);

        foreach (var s in stepsToUpdate)
        {
            s.StepNo -= 1;
        }

        await _db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return ApiResult<bool>.Ok(true, "Process step removed and reordered.");
    }
}

