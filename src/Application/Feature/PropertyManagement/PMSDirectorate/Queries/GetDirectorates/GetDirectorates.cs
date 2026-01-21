using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetDirectorates;
public record IdNameDto(Guid Id, string Name, string Code, string? ModuleName);
public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);

public record GetDirectoratesQuery() : IRequest<ApiResult<List<IdNameDto>>>;

public class GetDirectoratesHandler : IRequestHandler<GetDirectoratesQuery, ApiResult<List<IdNameDto>>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;

    public GetDirectoratesHandler(
        IPMSApplicationDbContext pmsDb,
        IApplicationDbContext appDb)
    {
        _pmsDb = pmsDb;
        _appDb = appDb;
    }

    public async Task<ApiResult<List<IdNameDto>>> Handle(GetDirectoratesQuery request, CancellationToken ct)
    {
        var modules = await _appDb.Modules
         .Select(m => new
         {
             m.Id,
             m.DisplayName
         })
         .ToListAsync(ct);

        var moduleLookup = modules
            .ToDictionary(x => x.Id, x => x.DisplayName);

        var list = await _pmsDb.Set<Directorate>()
        .Where(x => x.IsActive==true && x.IsDeleted !=true)
        .OrderBy(x => x.Name)
        .Select(x => new IdNameDto(
            x.Id,
            x.Name,
            x.Code,
            moduleLookup.ContainsKey(x.ModuleId)
                ? moduleLookup[x.ModuleId]
                : null
        ))
        .ToListAsync(ct);


        return ApiResult<List<IdNameDto>>.Ok(list);
    }
}

