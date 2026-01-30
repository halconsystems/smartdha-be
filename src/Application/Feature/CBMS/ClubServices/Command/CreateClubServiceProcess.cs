using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Command;

public record CreateClubServiceProcessCommand(
    Guid CategoryId,
    string Name,
    string Code,
    string? Description,
    string? ImageURL,
    FoodType? FoodType,
    string? Price,
    bool? IsAvailable,
    bool? IsPriceVisible,
    bool? Action,
    string? ActionName,
    string? ActionType,
    bool IsFeeAtSubmission,
    bool IsVoucherPossible,
    bool IsFeeRequired,
    bool IsfeeSubmit,
    bool IsInstructionAtStart,
    bool IsButton,
    string? Instruction
) : IRequest<ApiResult<Guid>>;

public class CreateClubServiceProcessCommandHandler : IRequestHandler<CreateClubServiceProcessCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _db;
    public CreateClubServiceProcessCommandHandler(IOLMRSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(CreateClubServiceProcessCommand r, CancellationToken ct)
    {
        var categoryExists = await _db.Set<Domain.Entities.CBMS.ClubCategories>().AnyAsync(x => x.Id == r.CategoryId, ct);
        if (!categoryExists) return ApiResult<Guid>.Fail("Club Category not found.");

        var exists = await _db.Set<Domain.Entities.CBMS.ClubServiceProcess>()
            .AnyAsync(x => x.CategoryId == r.CategoryId && x.Code == r.Code, ct);

        if (exists) return ApiResult<Guid>.Fail("CLub Service Process code already exists for this category.");

        var entity = new Domain.Entities.CBMS.ClubServiceProcess
        {
            CategoryId = r.CategoryId,
            Name = r.Name.Trim(),
            Code = r.Code.Trim().ToUpperInvariant(),
            Description = r.Description,
            FoodType = r.FoodType,
            Price = r.Price,
            IsAvailable = r.IsAvailable,
            IsPriceVisible = r.IsAvailable,
            Action = r.Action,
            ActionName = r.ActionName,
            ActionType = r.ActionType,
            IsFeeAtSubmission = r.IsFeeAtSubmission,
            IsVoucherPossible = r.IsVoucherPossible,
            IsFeeRequired = r.IsFeeRequired,
            IsfeeSubmit = r.IsfeeSubmit,
            IsInstructionAtStart = r.IsInstructionAtStart,
            Instruction = r.Instruction,
            IsButton = r.IsButton,
        };

        _db.Set<Domain.Entities.CBMS.ClubServiceProcess > ().Add(entity);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(entity.Id, "CLub Service Process created.");
    }
}

