using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Commands.CreateServiceProcess;
public record CreateServiceProcessCommand(
    Guid CategoryId,
    string Name,
    string Code,
    bool IsFeeAtSubmission,
    bool IsVoucherPossible,
    bool IsFeeRequired,
    bool IsNadraVerificationRequired,
    bool IsfeeSubmit,
    bool IsInstructionAtStart,
    string? Instruction
) : IRequest<ApiResult<Guid>>;

public class CreateServiceProcessHandler : IRequestHandler<CreateServiceProcessCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public CreateServiceProcessHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreateServiceProcessCommand r, CancellationToken ct)
    {
        var categoryExists = await _db.Set<ServiceCategory>().AnyAsync(x => x.Id == r.CategoryId, ct);
        if (!categoryExists) return ApiResult<Guid>.Fail("Category not found.");

        var exists = await _db.Set<ServiceProcess>()
            .AnyAsync(x => x.CategoryId == r.CategoryId && x.Code == r.Code, ct);

        if (exists) return ApiResult<Guid>.Fail("Process code already exists for this category.");

        var entity = new ServiceProcess
        {
            CategoryId = r.CategoryId,
            Name = r.Name.Trim(),
            Code = r.Code.Trim().ToUpperInvariant(),
            IsFeeAtSubmission = r.IsFeeAtSubmission,
            IsVoucherPossible = r.IsVoucherPossible,
            IsFeeRequired = r.IsFeeRequired,
            IsNadraVerificationRequired = r.IsNadraVerificationRequired,
            IsfeeSubmit = r.IsfeeSubmit,
            IsInstructionAtStart = r.IsInstructionAtStart,
            Instruction = r.Instruction
        };

        _db.Set<ServiceProcess>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "Process created.");
    }
}

