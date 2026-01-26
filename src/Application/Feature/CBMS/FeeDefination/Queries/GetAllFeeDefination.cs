using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeByProcess;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FeeDefination.Queries;

public record GetFeeDefinitionQuery()
    : IRequest<ApiResult<List<ClubFeeSetupDTO>>>;
public class GetFeeDefinitionHandler
    : IRequestHandler<GetFeeDefinitionQuery, ApiResult<List<ClubFeeSetupDTO>>>
{
    private readonly IPMSApplicationDbContext _db;
    public GetFeeDefinitionHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<ClubFeeSetupDTO>>> Handle(GetFeeDefinitionQuery r, CancellationToken ct)
    {
        var fee = await _db.Set<FeeDefinition>()
            .AsNoTracking()
            .ToListAsync(ct);

        if (fee == null)
            return ApiResult<List<ClubFeeSetupDTO>>.Fail("Fee definition not found.");


        var slabs = await _db.Set<FeeSlab>()
            .Where(x => fee.Select(f => f.Id).Contains(x.FeeDefinitionId))
            .OrderBy(x => x.FromArea)
            .Select(x => new
            {
                x.FeeDefinitionId,
                Slab = new ClubFeeSlabDto(
                    x.FromArea,
                    x.ToArea,
                    x.Amount
                )
            })
            .ToListAsync(ct);

        var result = fee.Select(f => new ClubFeeSetupDTO(
            f.Id,
            f.FeeType,
            f.FixedAmount,
            f.AreaUnit,
            f.AllowOverride,
            slabs
                .Where(s => s.FeeDefinitionId == f.Id)
                .Select(s => s.Slab)
                .ToList()
        )).ToList();

        return ApiResult<List<ClubFeeSetupDTO>>.Ok(result);
    }
}


