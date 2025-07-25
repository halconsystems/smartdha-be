using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Queries.GetUserModulePermissions;
public record GetUserModulePermissionsQuery(string UserId) : IRequest<UserModulePermissionsDto>;

public class GetUserModulePermissionsQueryHandler : IRequestHandler<GetUserModulePermissionsQuery, UserModulePermissionsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

    public GetUserModulePermissionsQueryHandler(IApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<UserModulePermissionsDto> Handle(GetUserModulePermissionsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user == null) throw new NotFoundException("User not found",request.UserId);

        var roleNames = await _userManager.GetRolesAsync(user);
        var primaryRole = roleNames.FirstOrDefault(); // Assuming single-role assignment

        // Get assigned modules
        // Ensure only non-null modules are loaded
        var modules = await _context.UserModuleAssignments
            .Where(x => x.UserId == request.UserId && x.Module != null)
            .Include(x => x.Module!)
                .ThenInclude(m => m.SubModules)
            .Select(x => x.Module!)
            .ToListAsync(cancellationToken);

        var result = new UserModulePermissionsDto();

        foreach (var module in modules)
        {
            var moduleDto = new ModuleDto
            {
                ModuleId = module.Id,
                ModuleName = module.Name
            };

            foreach (var sub in module.SubModules ?? new List<SubModule>())
            {
                var permission = await _context.RolePermissions
                    .Where(p => p.RoleName == primaryRole && p.SubModuleId == sub.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                moduleDto.SubModules.Add(new SubModuleDto
                {
                    SubModuleId = sub.Id,
                    SubModuleName = sub.Name,
                    CanRead = permission?.CanRead ?? false,
                    CanWrite = permission?.CanWrite ?? false,
                    CanDelete = permission?.CanDelete ?? false
                });
            }

            result.Modules.Add(moduleDto);
        }


        return result;
    }
}
