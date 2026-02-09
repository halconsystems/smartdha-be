using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.AddProcessStep.Commands;
public record AddProcessStepCommand(
    Guid ProcessId,
    int StepNo,
    string Name,
    Guid DirectorateId,
    bool RequiresVoucher,
    bool RequiresPaymentBeforeNext,
    int? SlaHours
) : IRequest<ApiResult<Guid>>;

public class AddProcessStepHandler : IRequestHandler<AddProcessStepCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public AddProcessStepHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(AddProcessStepCommand r, CancellationToken ct)
    {
        var processExists = await _db.Set<ServiceProcess>().AnyAsync(x => x.Id == r.ProcessId && x.IsDeleted != true, ct);
        if (!processExists) return ApiResult<Guid>.Fail("Process not found.");

        var dirExists = await _db.Set<Directorate>().AnyAsync(x => x.Id == r.DirectorateId && x.IsDeleted != true, ct);
        if (!dirExists) return ApiResult<Guid>.Fail("Directorate not found.");

        var dup = await _db.Set<ProcessStep>()
            .AnyAsync(x => x.ProcessId == r.ProcessId && x.StepNo == r.StepNo && x.IsDeleted != true, ct);
        if (dup) return ApiResult<Guid>.Fail("StepNo already exists for this process.");

        var step = new ProcessStep
        {
            ProcessId = r.ProcessId,
            StepNo = r.StepNo,
            Name = r.Name.Trim(),
            DirectorateId = r.DirectorateId,
            RequiresVoucher = r.RequiresVoucher,
            RequiresPaymentBeforeNext = r.RequiresPaymentBeforeNext,
            SlaHours = r.SlaHours
        };

        _db.Set<ProcessStep>().Add(step);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(step.Id, "Step added.");
    }
}

