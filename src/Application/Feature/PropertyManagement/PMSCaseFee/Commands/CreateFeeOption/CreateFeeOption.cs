using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.CreateFeeOption;
public record CreateFeeOptionCommand(
    Guid FeeDefinitionId,
    Guid? FeeCategoryId,   // NULL for OptionBased
    string Code,
    string Name,
    int ProcessingDays,
    decimal Amount,
    int SortOrder
) : IRequest<ApiResult<Guid>>;
public class CreateFeeOptionHandler
    : IRequestHandler<CreateFeeOptionCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;

    public CreateFeeOptionHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<Guid>> Handle(CreateFeeOptionCommand r, CancellationToken ct)
    {
        var def = await _db.Set<FeeDefinition>()
            .FirstOrDefaultAsync(x => x.Id == r.FeeDefinitionId, ct);

        if (def == null)
            return ApiResult<Guid>.Fail("Fee definition not found.");

        var option = new FeeOption
        {
            FeeDefinitionId = r.FeeDefinitionId,
            FeeCategoryId = r.FeeCategoryId,
            Code = r.Code.Trim().ToUpperInvariant(),
            Name = r.Name.Trim(),
            ProcessingDays = r.ProcessingDays,
            Amount = r.Amount,
            SortOrder = r.SortOrder
        };

        _db.Set<FeeOption>().Add(option);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(option.Id, "Fee option added.");
    }
}

