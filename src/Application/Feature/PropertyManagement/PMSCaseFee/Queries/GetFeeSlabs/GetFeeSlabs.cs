using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeByProcess;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeSlabs;
public record GetFeeSlabsQuery(Guid FeeDefinitionId)
    : IRequest<ApiResult<List<FeeSlabDto>>>;

public class GetFeeSlabsHandler
    : IRequestHandler<GetFeeSlabsQuery, ApiResult<List<FeeSlabDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    public GetFeeSlabsHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<FeeSlabDto>>> Handle(GetFeeSlabsQuery r, CancellationToken ct)
    {
        var slabs = await _db.Set<FeeSlab>()
            .Where(x => x.FeeDefinitionId == r.FeeDefinitionId)
            .OrderBy(x => x.FromArea)
            .Select(x => new FeeSlabDto(x.FromArea, x.ToArea, x.Amount))
            .ToListAsync(ct);

        return ApiResult<List<FeeSlabDto>>.Ok(slabs);
    }
}

