using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
public record GetAccessTreeQuery : IRequest<SuccessResponse<List<ModuleTreeDto>>>;

public class GetAccessTreeQueryHandler : IRequestHandler<GetAccessTreeQuery, SuccessResponse<List<ModuleTreeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public GetAccessTreeQueryHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUser)
    {
        _context = context;
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<List<ModuleTreeDto>>> Handle(GetAccessTreeQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
            throw new UnAuthorizedException("Invalid user context.");

        // Try to load user (optional if you only need roles)
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new UnAuthorizedException("User not found.");

        // Now get roles safely
        var roles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId.ToString()) // use userId directly instead of user.Id
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);


        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        // Step 1: Get all modules/submodules/permissions
        var allModules = await _context.Modules
            .Include(m => m.SubModules)
                .ThenInclude(sm => sm.Permissions)
            .ToListAsync(ct);

        // Step 2: Get current user assigned permissions (if not superadmin)
        var userPermissions = new List<AppRolePermission>();

        if (!isSuperAdmin)
        {
            userPermissions = await _context.AppRolePermissions
                .Where(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId.ToString()))
                .ToListAsync(ct);
        }

        // Step 3: Build tree
        var result = allModules.Select(m => new ModuleTreeDto
        {
            Id = m.Id,
            Name = m.DisplayName,
            Checked = isSuperAdmin || userPermissions.Any(p => p.SubModule.ModuleId == m.Id),
            SubModules = m.SubModules.Select(sm => new SubModuleTreeDto
            {
                Id = sm.Id,
                Name = sm.DisplayName,
                Checked = isSuperAdmin || userPermissions.Any(p => p.SubModuleId == sm.Id),
                Permissions = sm.Permissions.Select(p => new PermissionTreeDto
                {
                    Id = p.Id,
                    Name = p.DisplayName,
                    Checked = isSuperAdmin || userPermissions.Any(up => up.SubModuleId == sm.Id &&
                        up.AllowedActions.Contains(p.Value)) // validate by CSV/JSON AllowedActions
                }).ToList()
            }).ToList()
        }).ToList();

        return new SuccessResponse<List<ModuleTreeDto>>(result);
    }
}

