using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserAccessById;

public record GetUserAccessByIdQuery(string UserId)
    : IRequest<SuccessResponse<UserAccessViewDto>>;
public class GetUserAccessByIdHandler
    : IRequestHandler<GetUserAccessByIdQuery, SuccessResponse<UserAccessViewDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserAccessByIdHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<SuccessResponse<UserAccessViewDto>> Handle(GetUserAccessByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException("User not found", request.UserId);

        var assignedModuleIds = await _context.UserModuleAssignments
            .Where(x => x.UserId == user.Id)
            .Select(x => x.ModuleId)
            .ToListAsync(cancellationToken);

        var assignedSubModuleIds = await _context.UserPermissions
            .Where(up => up.UserId == user.Id)
            .Select(up => up.SubModuleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var userPermissions = await _context.UserPermissions
            .Where(up => up.UserId == user.Id)
            .ToListAsync(cancellationToken);

        var modules = await _context.Modules
            .Include(m => m.SubModules)
                .ThenInclude(sm => sm.Permissions)
            .ToListAsync(cancellationToken);

        var dto = new UserAccessViewDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email ?? "",
            CNIC = user.CNIC,
            MobileNo = user.MobileNo,
            Role = (await _context.AppUserRoles
                        .Include(ur => ur.Role)
                        .Where(ur => ur.UserId == user.Id)
                        .Select(ur => ur.Role.Name)
                        .FirstOrDefaultAsync(cancellationToken)) ?? "User",

            Modules = modules.Select(m => new ModuleAccessDto
            {
                ModuleId = m.Id,
                ModuleName = m.DisplayName,
                IsAlreadyAssigned = assignedModuleIds.Contains(m.Id),
                SubModules = m.SubModules.Select(sm => new SubModuleAccessDto
                {
                    SubModuleId = sm.Id,
                    SubModuleName = sm.DisplayName,
                    RequiresPermission=sm.RequiresPermission,
                    IsAlreadyAssigned = assignedSubModuleIds.Contains(sm.Id),
                    Permissions = sm.Permissions.Select(p => new PermissionAccessDto
                    {
                        PermissionId = p.Id,
                        DisplayName = p.DisplayName,
                        Value = p.Value,
                        IsAlreadyAssigned = userPermissions.Any(up =>
                            up.SubModuleId == sm.Id &&
                            up.AllowedActions.Contains(p.Id.ToString()))
                    }).ToList()
                }).ToList()
            }).ToList()
        };

        return new SuccessResponse<UserAccessViewDto>(dto, "User access info fetched successfully");
    }
}

