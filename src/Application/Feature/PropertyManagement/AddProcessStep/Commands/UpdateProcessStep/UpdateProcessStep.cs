using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.AddProcessStep.Commands.UpdateProcessStep;
public record UpdateProcessStepCommand(
    Guid StepId,
    int NewStepNo,
    string Name,
    Guid DirectorateId,
    bool RequiresVoucher,
    bool RequiresPaymentBeforeNext,
    int? SlaHours
) : IRequest<ApiResult<bool>>;
public class UpdateProcessStepHandler
    : IRequestHandler<UpdateProcessStepCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public UpdateProcessStepHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateProcessStepCommand r,
        CancellationToken ct)
    {
        using var trx = await _db.Database.BeginTransactionAsync(ct);

        // 1️⃣ Load step
        var step = await _db.Set<ProcessStep>()
            .FirstOrDefaultAsync(x =>
                x.Id == r.StepId &&
                x.IsDeleted != true,
                ct);

        if (step == null)
            return ApiResult<bool>.Fail("Process step not found.");

        // 2️⃣ Validate directorate
        var dirExists = await _db.Set<Directorate>()
            .AnyAsync(x => x.Id == r.DirectorateId && x.IsDeleted != true, ct);

        if (!dirExists)
            return ApiResult<bool>.Fail("Directorate not found.");

        var oldStepNo = step.StepNo;
        var newStepNo = r.NewStepNo;

        // 3️⃣ Reorder only if StepNo changed
        if (oldStepNo != newStepNo)
        {
            var steps = await _db.Set<ProcessStep>()
                .Where(x =>
                    x.ProcessId == step.ProcessId &&
                    x.IsDeleted != true)
                .ToListAsync(ct);

            if (newStepNo < 1 || newStepNo > steps.Count)
                return ApiResult<bool>.Fail("Invalid step number.");

            // 🔽 Moving DOWN (e.g. 2 → 5)
            if (newStepNo > oldStepNo)
            {
                foreach (var s in steps.Where(x =>
                    x.StepNo > oldStepNo &&
                    x.StepNo <= newStepNo))
                {
                    s.StepNo--;
                }
            }
            // 🔼 Moving UP (e.g. 5 → 2)
            else
            {
                foreach (var s in steps.Where(x =>
                    x.StepNo >= newStepNo &&
                    x.StepNo < oldStepNo))
                {
                    s.StepNo++;
                }
            }

            step.StepNo = newStepNo;
        }

        // 4️⃣ Update other fields
        step.Name = r.Name.Trim();
        step.DirectorateId = r.DirectorateId;
        step.RequiresVoucher = r.RequiresVoucher;
        step.RequiresPaymentBeforeNext = r.RequiresPaymentBeforeNext;
        step.SlaHours = r.SlaHours;

        await _db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return ApiResult<bool>.Ok(true, "Step updated successfully.");
    }
}
