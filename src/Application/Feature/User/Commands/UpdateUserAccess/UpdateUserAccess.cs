using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterUser;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.UpdateUserAccess;
public record UpdateUserAccessCommand : IRequest<SuccessResponse<Guid>>
{
    public string UserId { get; set; } = default!;   // existing user
    public List<ModuleSelectionDto> Modules { get; set; } = new();
}
public class UpdateUserAccessCommandHandler
    : IRequestHandler<UpdateUserAccessCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserAccessCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateUserAccessCommand request, CancellationToken cancellationToken)
    {
        // validate user exists
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException("User not found", request.UserId);

        // 🧹 Step 1: Remove old assignments
        var oldModules = _context.UserModuleAssignments.Where(x => x.UserId == request.UserId);
        var oldPermissions = _context.UserPermissions.Where(x => x.UserId == request.UserId);

        _context.UserModuleAssignments.RemoveRange(oldModules);
        _context.UserPermissions.RemoveRange(oldPermissions);

        // ✅ Step 2: Insert new assignments
        foreach (var moduleSel in request.Modules)
        {
            // User ↔ Module link
            var assignment = new UserModuleAssignment
            {
                UserId = user.Id,
                ModuleId = moduleSel.ModuleId
            };
            _context.UserModuleAssignments.Add(assignment);

            if (moduleSel.SubModules != null)
            {
                foreach (var subSel in moduleSel.SubModules)
                {
                    if (subSel.PermissionIds != null && subSel.PermissionIds.Any())
                    {
                        // With permissions
                        var userPermission = new UserPermission
                        {
                            UserId = user.Id,
                            SubModuleId = subSel.SubModuleId,
                            AllowedActions = string.Join(",", subSel.PermissionIds)
                        };
                        _context.UserPermissions.Add(userPermission);
                    }
                    else
                    {
                        // No permissions, just mark submodule access
                        var userPermission = new UserPermission
                        {
                            UserId = user.Id,
                            SubModuleId = subSel.SubModuleId,
                            AllowedActions = string.Empty
                        };
                        _context.UserPermissions.Add(userPermission);
                    }
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // return Guid (parsed from string Id)
        var userGuid = Guid.TryParse(user.Id, out var parsedGuid) ? parsedGuid : Guid.Empty;
        return new SuccessResponse<Guid>(userGuid, "User access updated successfully.");
    }
}

