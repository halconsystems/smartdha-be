using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.UpdateServiceProcess;
public record UpdateServiceProcessCommand(
    Guid Id,
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
) : IRequest<ApiResult<bool>>;

public class UpdateServiceProcessHandler
    : IRequestHandler<UpdateServiceProcessCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public UpdateServiceProcessHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(UpdateServiceProcessCommand r, CancellationToken ct)
    {
        var entity = await _db.Set<ServiceProcess>()
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Process not found.");

        var categoryExists = await _db.Set<ServiceCategory>()
            .AnyAsync(x => x.Id == r.CategoryId, ct);

        if (!categoryExists)
            return ApiResult<bool>.Fail("Category not found.");

        var duplicateCode = await _db.Set<ServiceProcess>()
            .AnyAsync(x =>
                x.Id != r.Id &&
                x.CategoryId == r.CategoryId &&
                x.Code == r.Code,
                ct);

        if (duplicateCode)
            return ApiResult<bool>.Fail("Process code already exists for this category.");

        // 🔹 Update fields
        entity.CategoryId = r.CategoryId;
        entity.Name = r.Name.Trim();
        entity.Code = r.Code.Trim().ToUpperInvariant();
        entity.IsFeeAtSubmission = r.IsFeeAtSubmission;
        entity.IsVoucherPossible = r.IsVoucherPossible;
        entity.IsFeeRequired = r.IsFeeRequired;
        entity.IsNadraVerificationRequired = r.IsNadraVerificationRequired;
        entity.IsfeeSubmit = r.IsfeeSubmit;
        entity.IsInstructionAtStart = r.IsInstructionAtStart;
        entity.Instruction = r.Instruction;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Process updated successfully.");
    }
}

