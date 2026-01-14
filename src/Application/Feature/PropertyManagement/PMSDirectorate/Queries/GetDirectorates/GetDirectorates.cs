using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetDirectorates;
public record IdNameDto(Guid Id, string Name, string Code);
public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);

public record GetDirectoratesQuery() : IRequest<ApiResult<List<IdNameDto>>>;

public class GetDirectoratesHandler : IRequestHandler<GetDirectoratesQuery, ApiResult<List<IdNameDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    public GetDirectoratesHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<IdNameDto>>> Handle(GetDirectoratesQuery request, CancellationToken ct)
    {
        var list = await _db.Set<Directorate>()
            .OrderBy(x => x.Name)
            .Select(x => new IdNameDto(x.Id, x.Name, x.Code))
            .ToListAsync(ct);

        return ApiResult<List<IdNameDto>>.Ok(list);
    }
}

