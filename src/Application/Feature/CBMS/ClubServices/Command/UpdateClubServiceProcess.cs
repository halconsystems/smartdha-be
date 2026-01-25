using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Command;

public record UpdateClubServiceProcess(
    Guid Id,
    Guid CategoryId,
    string Name,
    string Code,
    string? Description,
    bool IsFeeAtSubmission,
    bool IsVoucherPossible,
    bool IsFeeRequired,
    bool IsfeeSubmit,
    bool IsInstructionAtStart,
    string? Instruction,
    bool IsButton
) : IRequest<ApiResult<bool>>;
public class UpdateClubServiceProcessHandler
    : IRequestHandler<UpdateClubServiceProcess, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public UpdateClubServiceProcessHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        UpdateClubServiceProcess request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.ClubServiceProcess>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Club Service not found.");

        var categoryExists = await _db.Set<ServiceCategory>().AnyAsync(x => x.Id == request.CategoryId, ct);
        if (!categoryExists) return ApiResult<bool>.Fail("Club Category not found.");


        var codeExists = await _db.Set<Domain.Entities.CBMS.ClubServiceProcess>()
            .AnyAsync(x => x.Code == request.Code && x.Id != request.Id, ct);

        if (codeExists)
            return ApiResult<bool>.Fail("Club Service Process code already exists.");

        entity.Name = request.Name.Trim();
        entity.Code = request.Code.Trim().ToUpperInvariant();
        entity.Description = request.Description;
        entity.CategoryId = request.CategoryId;
        entity.IsFeeAtSubmission = request.IsFeeAtSubmission;
        entity.IsFeeRequired = request.IsFeeRequired;
        entity.IsVoucherPossible = request.IsVoucherPossible;
        entity.IsfeeSubmit = request.IsfeeSubmit;
        entity.Instruction = request.Instruction;
        entity.IsInstructionAtStart = request.IsInstructionAtStart;
        entity.IsButton = request.IsButton;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Club Service Process updated successfully.");
    }
}

