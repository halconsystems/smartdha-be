using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions.Dtos;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions;

public record UpdateUserModulePermissionsCommand(string UserId, List<ModulePermissionDto> Modules) : IRequest<string>;

public class UpdateUserModulePermissionsCommandHandler : IRequestHandler<UpdateUserModulePermissionsCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UpdateUserModulePermissionsCommandHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<string> Handle(UpdateUserModulePermissionsCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                   ?? throw new NotFoundException("User not found");

        var primaryRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                  ?? throw new Exception("User has no role assigned");

        foreach (var module in request.Modules)
        {
            var assignmentExists = await _context.UserModuleAssignments
                .AnyAsync(x => x.UserId == request.UserId && x.ModuleId == module.ModuleId, cancellationToken);

            if (!assignmentExists)
            {
                var assignment = _mapper.Map<UserModuleAssignment>(module);
                assignment.Id = Guid.NewGuid();
                assignment.UserId = request.UserId;
                _context.UserModuleAssignments.Add(assignment);
            }

            foreach (var sub in module.SubModules)
            {
                var existingPermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp =>
                        rp.RoleName == primaryRole &&
                        rp.SubModuleId == sub.SubModuleId, cancellationToken);

                if (existingPermission == null)
                {
                    var newPermission = _mapper.Map<AppRolePermission>(sub);
                    newPermission.Id = Guid.NewGuid();
                    newPermission.RoleName = primaryRole;
                    _context.RolePermissions.Add(newPermission);
                }
                else if (
                    existingPermission.CanRead != sub.CanRead ||
                    existingPermission.CanWrite != sub.CanWrite ||
                    existingPermission.CanDelete != sub.CanDelete)
                {
                    _mapper.Map(sub, existingPermission); // update values
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return "Permissions updated successfully.";
    }
}
