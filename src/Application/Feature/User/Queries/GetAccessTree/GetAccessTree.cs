using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
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

        // Try to load user
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new UnAuthorizedException("User not found.");

        // Load user roles
        var roles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId.ToString())
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        List<Module> modules;

        if (isSuperAdmin)
        {
            // ✅ SuperAdmin → load ALL modules with all submodules and permissions
            modules = await _context.Modules
                .Where(m => m.AppType == AppType.Web)
                .Include(m => m.SubModules)
                    .ThenInclude(sm => sm.Permissions)
                .ToListAsync(ct);
        }
        else
        {
            // ✅ Normal user → load only assigned modules
            var userModuleIds = await _context.UserModuleAssignments
                .Where(x => x.UserId == user.Id)
                .Select(x => x.ModuleId)
                .ToListAsync(ct);

            // user assigned permission Ids
            var userPermissionIds = await _context.UserPermissionAssignments
                .Where(up => up.UserId == user.Id)
                .Select(up => up.PermissionId)
                .ToListAsync(ct);

            modules = await _context.Modules
                .Where(m => userModuleIds.Contains(m.Id) && m.AppType == AppType.Web)
                .Include(m => m.SubModules)
                    .ThenInclude(sm => sm.Permissions)
                .ToListAsync(ct);

            // Filter out unassigned permissions
            foreach (var module in modules)
            {
                foreach (var sub in module.SubModules)
                {
                    sub.Permissions = sub.Permissions
                        .Where(p => userPermissionIds.Contains(p.Id))
                        .ToList();
                }

                // Optionally remove submodules that end up with no permissions
                module.SubModules = module.SubModules
                    .Where(sm => sm.Permissions.Any())
                    .ToList();
            }
        }

        // Build tree
        var result = modules.Select(m => new ModuleTreeDto
        {
            Id = m.Id,
            Name = m.DisplayName,
            Value = m.Value,
            DisplayName = m.DisplayName,
            SubModules = m.SubModules.Select(sm => new SubModuleTreeDto
            {
                Id = sm.Id,
                Name = sm.DisplayName,
                Value = sm.Value,
                DisplayName = sm.DisplayName,
                Permissions = sm.Permissions.Select(p => new PermissionTreeDto
                {
                    Id = p.Id,
                    Name = p.DisplayName,
                    Value = p.Value,
                    DisplayName = p.DisplayName
                }).ToList()
            }).ToList()
        }).ToList();

        return new SuccessResponse<List<ModuleTreeDto>>(result);
    }

}

