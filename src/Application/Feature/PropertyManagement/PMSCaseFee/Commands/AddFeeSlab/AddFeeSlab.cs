using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands.AddFeeSlab;
public record AddFeeSlabCommand(
    Guid FeeDefinitionId,
    decimal FromArea,
    decimal ToArea,
    decimal Amount
) : IRequest<ApiResult<Guid>>;
public class AddFeeSlabHandler
    : IRequestHandler<AddFeeSlabCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    public AddFeeSlabHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<Guid>> Handle(AddFeeSlabCommand r, CancellationToken ct)
    {
        var feeExists = await _db.Set<FeeDefinition>()
            .AnyAsync(x => x.Id == r.FeeDefinitionId, ct);

        if (!feeExists)
            return ApiResult<Guid>.Fail("Fee definition not found.");

        var slab = new FeeSlab
        {
            FeeDefinitionId = r.FeeDefinitionId,
            FromArea = r.FromArea,
            ToArea = r.ToArea,
            Amount = r.Amount
        };

        _db.Set<FeeSlab>().Add(slab);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(slab.Id, "Fee slab added.");
    }
}

