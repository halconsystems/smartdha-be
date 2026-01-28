using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetDirectorates;
public record IdNameDto(Guid Id, string Name, string Code, string? ModuleName, string? RoleName);
public record DirectorateIdNameDto(Guid Id, string Name, string Code, string? ModuleName, string? RoleName,Guid ModuleId, Guid RoleId);
public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);

public record GetDirectoratesQuery() : IRequest<ApiResult<List<DirectorateIdNameDto>>>;

public class GetDirectoratesHandler : IRequestHandler<GetDirectoratesQuery, ApiResult<List<DirectorateIdNameDto>>>
{
    private readonly IPMSApplicationDbContext _pmsDb;
    private readonly IApplicationDbContext _appDb;
    private readonly IRoleService _roleService;

    public GetDirectoratesHandler(
        IPMSApplicationDbContext pmsDb,
        IApplicationDbContext appDb,
        IRoleService roleService)
    {
        _pmsDb = pmsDb;
        _appDb = appDb;
        _roleService = roleService;
    }

    public async Task<ApiResult<List<DirectorateIdNameDto>>> Handle(GetDirectoratesQuery request, CancellationToken ct)
    {
        var directorates = await _pmsDb.Set<Directorate>()
        .Where(x => x.IsActive == true && x.IsDeleted != true)
        .OrderBy(x => x.Name)
        .Select(x => new
        {
            x.Id,
            x.Name,
            x.Code,
            x.ModuleId,
            x.RoleId
        })
        .ToListAsync(ct);

        // 2️⃣ Modules lookup
        var moduleLookup = await _appDb.Modules
            .Select(m => new { m.Id, m.DisplayName })
            .ToDictionaryAsync(x => x.Id, x => x.DisplayName, ct);

        // 3️⃣ Roles lookup (bulk, no service call)
        var roleIds = directorates.Select(x => x.RoleId).Distinct().ToList();

        var roleLookup = await _appDb.AppRoles
            .Where(r => roleIds.Contains(r.Id)
                        && (r.IsDeleted == null || r.IsDeleted == false))
            .Select(r => new { r.Id, r.Name })
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        // 4️⃣ Map to DTO
        var list = directorates.Select(x => new DirectorateIdNameDto(
            x.Id,
            x.Name,
            x.Code,
            moduleLookup.TryGetValue(x.ModuleId, out var moduleName)
                ? moduleName
                : null,
            roleLookup.TryGetValue(x.RoleId, out var roleName)
                ? roleName
                : null,
            x.ModuleId,
            x.RoleId
        )).ToList();

        return ApiResult<List<DirectorateIdNameDto>>.Ok(list);
    }
}

