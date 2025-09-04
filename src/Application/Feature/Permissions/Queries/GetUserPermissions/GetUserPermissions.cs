using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Permissions.Queries.GetUserPermissions;
public record GetUserPermissionsQuery(string UserId) : IRequest<List<string>>;

public class GetUserPermissionsQueryHandler
    : IRequestHandler<GetUserPermissionsQuery, List<string>>
{
    private readonly IApplicationDbContext _ctx;

    public GetUserPermissionsQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<List<string>> Handle(GetUserPermissionsQuery request, CancellationToken ct)
    {
        var userModuleIds = await _ctx.UserModuleAssignments
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.ModuleId)
            .ToListAsync(ct);

        var userSubModuleIds = await _ctx.UserSubModuleAssignments
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.SubModuleId)
            .ToListAsync(ct);

        var userPermissionIds = await _ctx.UserPermissionAssignments
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.PermissionId)
            .ToListAsync(ct);

        var modules = await _ctx.Modules
            .Where(m => userModuleIds.Contains(m.Id))
            .Include(m => m.SubModules)
                .ThenInclude(sm => sm.Permissions)
            .ToListAsync(ct);

        var flat = new List<string>();
        foreach (var module in modules)
        {
            flat.Add(module.Value); // module-level

            foreach (var sm in module.SubModules)
            {
                if (userSubModuleIds.Contains(sm.Id))
                {
                    flat.Add(sm.Value); // submodule-level

                    foreach (var perm in sm.Permissions)
                    {
                        if (userPermissionIds.Contains(perm.Id))
                        {
                            flat.Add($"{sm.Value}.{perm.Value}");
                        }
                    }
                }
            }
        }
        return flat;
    }
}

