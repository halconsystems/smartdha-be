using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFeeOption.Command;

public record CreateClubFeeOptionCommand(
    Guid FeeDefinitionId,
    Guid? FeeCategoryId,   // NULL for OptionBased
    string Code,
    string Name,
    int ProcessingDays,
    decimal Amount,
    int SortOrder
) : IRequest<ApiResult<Guid>>;
public class CreateClubFeeOptionCommandHandler
    : IRequestHandler<CreateClubFeeOptionCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public CreateClubFeeOptionCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(CreateClubFeeOptionCommand r, CancellationToken ct)
    {
        var def = await _db.Set<Domain.Entities.CBMS.ClubFeeOption>()
            .FirstOrDefaultAsync(x => x.Id == r.FeeDefinitionId, ct);

        if (def == null)
            return ApiResult<Guid>.Fail("Fee definition not found.");

        var option = new Domain.Entities.CBMS.ClubFeeOption
        {
            FeeDefinitionId = r.FeeDefinitionId,
            FeeCategoryId = r.FeeCategoryId,
            Code = r.Code.Trim().ToUpperInvariant(),
            Name = r.Name.Trim(),
            ProcessingDays = r.ProcessingDays,
            Amount = r.Amount,
            SortOrder = r.SortOrder
        };

        _db.Set<Domain.Entities.CBMS.ClubFeeOption>().Add(option);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(option.Id, "Fee option added.");
    }
}

