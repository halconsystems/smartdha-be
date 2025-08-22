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

    public GetUserPermissionsQueryHandler(IApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<string>> Handle(GetUserPermissionsQuery request, CancellationToken ct)
    {
        var permissions = await _ctx.UserPermissions
            .Include(up => up.SubModule)
            .ThenInclude(sm => sm.Permissions)
            .Where(up => up.UserId == request.UserId)
            .ToListAsync(ct);

        // flatten: SubModule.Action
        return permissions.SelectMany(up =>
            up.AllowedActions.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(action => $"{up.SubModule.Value}.{action.Trim()}")
        ).ToList();
    }
}

