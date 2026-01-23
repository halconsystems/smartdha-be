using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetDirectorates;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries.GetFeeCategories;
public record GetFeeCategoriesQuery()
    : IRequest<ApiResult<List<IdNameDto>>>;
public class GetFeeCategoriesHandler
    : IRequestHandler<GetFeeCategoriesQuery, ApiResult<List<IdNameDto>>>
{
    private readonly IPMSApplicationDbContext _db;

    public GetFeeCategoriesHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<List<IdNameDto>>> Handle(
        GetFeeCategoriesQuery r, CancellationToken ct)
    {
        var list = await _db.Set<FeeCategory>()
            .OrderBy(x => x.Code)
            .Select(x => new IdNameDto(x.Id, x.Name, x.Code, ""))
            .ToListAsync(ct);

        return ApiResult<List<IdNameDto>>.Ok(list);
    }
}

