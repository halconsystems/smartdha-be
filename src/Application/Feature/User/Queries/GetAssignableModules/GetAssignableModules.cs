using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetAssignableModules;
public record GetAssignableModulesQuery(string? CurrentUserId) : IRequest<List<ModuleWithSubDto>>;

public class GetAssignableModulesHandler : IRequestHandler<GetAssignableModulesQuery, List<ModuleWithSubDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public GetAssignableModulesHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<List<ModuleWithSubDto>> Handle(GetAssignableModulesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .Include(u => u.ModuleAssignments)
            .FirstOrDefaultAsync(u => u.Id == request.CurrentUserId, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        List<Module> modules;

        if (roles.Contains(AllRoles.SuperAdministrator))
        {
            modules = await _context.Modules
                .Include(m => m.SubModules)
                .ToListAsync(cancellationToken);
        }
        else
        {
            var moduleIds = user.ModuleAssignments.Select(a => a.ModuleId).ToList();

            modules = await _context.Modules
                .Where(m => moduleIds.Contains(m.Id))
                .Include(m => m.SubModules)
                .ToListAsync(cancellationToken);
        }

        return modules.Select(m => new ModuleWithSubDto
        {
            ModuleId = m.Id,
            ModuleName = m.Name,
            Description = m.Description,
            SubModules = m.SubModules.Select(sm => new SubModuleDto
            {
                SubModuleId = sm.Id,
                SubModuleName = sm.Name,
                Description= sm.Description
            }).ToList()
        }).ToList();
    }
}
