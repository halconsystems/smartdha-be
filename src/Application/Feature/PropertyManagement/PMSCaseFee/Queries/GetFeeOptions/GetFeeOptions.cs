using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeOptions;
public record GetFeeOptionsQuery(Guid FeeDefinitionId)
    : IRequest<ApiResult<List<FeeOptionDto>>>;
public class GetFeeOptionsHandler
    : IRequestHandler<GetFeeOptionsQuery, ApiResult<List<FeeOptionDto>>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetFeeOptionsHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<FeeOptionDto>>> Handle(
        GetFeeOptionsQuery r, CancellationToken ct)
    {
        var list = await _db.Set<FeeOption>()
            .Where(x => x.FeeDefinitionId == r.FeeDefinitionId)
            .OrderBy(x => x.SortOrder)
            .Select(x => new FeeOptionDto(
                x.Id,
                x.Code,
                x.Name,
                x.ProcessingDays,
                x.Amount
            ))
            .ToListAsync(ct);

        return ApiResult<List<FeeOptionDto>>.Ok(list);
    }
}

