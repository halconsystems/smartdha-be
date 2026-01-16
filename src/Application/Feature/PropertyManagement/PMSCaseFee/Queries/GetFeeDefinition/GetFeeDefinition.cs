using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeByProcess;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeDefinition;
public record GetFeeDefinitionQuery(Guid FeeDefinitionId)
    : IRequest<ApiResult<FeeSetupDto>>;
public class GetFeeDefinitionHandler
    : IRequestHandler<GetFeeDefinitionQuery, ApiResult<FeeSetupDto>>
{
    private readonly IPMSApplicationDbContext _db;
    public GetFeeDefinitionHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<FeeSetupDto>> Handle(GetFeeDefinitionQuery r, CancellationToken ct)
    {
        var fee = await _db.Set<FeeDefinition>()
            .FirstOrDefaultAsync(x => x.Id == r.FeeDefinitionId, ct);

        if (fee == null)
            return ApiResult<FeeSetupDto>.Fail("Fee definition not found.");

        var slabs = await _db.Set<FeeSlab>()
            .Where(x => x.FeeDefinitionId == fee.Id)
            .OrderBy(x => x.FromArea)
            .Select(x => new FeeSlabDto(x.FromArea, x.ToArea, x.Amount))
            .ToListAsync(ct);

        return ApiResult<FeeSetupDto>.Ok(new FeeSetupDto(
            fee.Id,
            fee.FeeType,
            fee.FixedAmount,
            fee.AreaUnit,
            fee.AllowOverride,
            slabs
        ));
    }
}

