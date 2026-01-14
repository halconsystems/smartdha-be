using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSServiceProcess.Queries.GetProcessesByCategory;
public record ProcessDto(Guid Id, Guid CategoryId, string Name, string Code);
public record GetProcessesByCategoryQuery(Guid CategoryId) : IRequest<ApiResult<List<ProcessDto>>>;

public class GetProcessesByCategoryHandler : IRequestHandler<GetProcessesByCategoryQuery, ApiResult<List<ProcessDto>>>
{
    private readonly IPMSApplicationDbContext _db;
    public GetProcessesByCategoryHandler(IPMSApplicationDbContext db) => _db = db;

    public async Task<ApiResult<List<ProcessDto>>> Handle(GetProcessesByCategoryQuery q, CancellationToken ct)
    {
        var list = await _db.Set<ServiceProcess>()
            .Where(x => x.CategoryId == q.CategoryId)
            .OrderBy(x => x.Name)
            .Select(x => new ProcessDto(x.Id, x.CategoryId, x.Name, x.Code))
            .ToListAsync(ct);

        return ApiResult<List<ProcessDto>>.Ok(list);
    }
}
