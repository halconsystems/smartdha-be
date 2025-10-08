using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterUser;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.UpdateUser;
public record UpdateUserCommand : IRequest<SuccessResponse<Guid>>
{
    public Guid Id { get; set; }               // which user to update
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public List<ModuleSelectionDto> Modules { get; set; } = new();
}
public class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1️⃣ Find existing user
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                throw new NotFoundException($"User with Id {request.Id} not found.");

            // 2️⃣ Update basic fields
            user.Name = request.Name;
            user.UserName = request.UserName;
            user.NormalizedUserName = request.UserName.ToUpper();
            user.Email = request.Email;
            user.NormalizedEmail = request.Email.ToUpper();
            user.MobileNo = request.MobileNo;
            user.CNIC = request.CNIC;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                throw new DBOperationException($"User update failed: {errors}");
            }

            // 3️⃣ Update Role
            if (!Guid.TryParse(request.RoleId, out var roleId))
                throw new DBOperationException("RoleId is not a valid GUID.");

            // remove existing roles
            var existingRoles = _context.AppUserRoles.Where(r => r.UserId == user.Id);
            _context.AppUserRoles.RemoveRange(existingRoles);

            var role = await _context.AppRoles.FindAsync(new object[] { roleId }, cancellationToken);
            if (role == null)
                throw new NotFoundException($"Role with ID {roleId} was not found.");

            _context.AppUserRoles.Add(new AppUserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            // 4️⃣ Clear old assignments
            // Remove only assignment rows, not the base entities
            var oldModules = await _context.UserModuleAssignments
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            var oldSubModules = await _context.UserSubModuleAssignments
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            var oldPermissions = await _context.UserPermissionAssignments
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            var oldClubs = await _context.UserClubAssignments
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            // Remove only the assignment rows
            // Soft delete instead of RemoveRange
            await _context.UserModuleAssignments
            .Where(x => x.UserId == user.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsDeleted, true), cancellationToken);

            await _context.UserSubModuleAssignments
                .Where(x => x.UserId == user.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(sm => sm.IsDeleted, true), cancellationToken);

            await _context.UserPermissionAssignments
                .Where(x => x.UserId == user.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true), cancellationToken);

            await _context.UserClubAssignments
                .Where(x => x.UserId == user.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsDeleted, true), cancellationToken);


            // 5️⃣ Reassign Modules + SubModules + Permissions + Clubs
            if (request.Modules != null)
            {
                foreach (var moduleSel in request.Modules)
                {
                    var module = await _context.Modules
                        .FirstOrDefaultAsync(m => m.Id == moduleSel.ModuleId, cancellationToken);

                    if (module == null)
                        throw new NotFoundException($"Module with Id {moduleSel.ModuleId} does not exist.");

                    _context.UserModuleAssignments.Add(new UserModuleAssignment
                    {
                        UserId = user.Id,
                        ModuleId = moduleSel.ModuleId
                    });

                    // ✅ Special Case: Club module
                    if (module.Value == "Club" || module.Value == "Ground.Club")
                    {
                        if (moduleSel.AssignedClubIds == null || !moduleSel.AssignedClubIds.Any())
                            throw new DBOperationException("At least one Club must be assigned for Club module.");

                        foreach (var clubId in moduleSel.AssignedClubIds)
                        {
                            _context.UserClubAssignments.Add(new UserClubAssignment
                            {
                                UserId = user.Id,
                                ClubId = clubId
                            });
                        }
                    }

                    // SubModules + Permissions
                    if (moduleSel.SubModules != null)
                    {
                        foreach (var subSel in moduleSel.SubModules)
                        {
                            _context.UserSubModuleAssignments.Add(new UserSubModuleAssignment
                            {
                                UserId = user.Id,
                                SubModuleId = subSel.SubModuleId
                            });

                            if (subSel.PermissionIds != null && subSel.PermissionIds.Any())
                            {
                                foreach (var permId in subSel.PermissionIds)
                                {
                                    _context.UserPermissionAssignments.Add(new UserPermissionAssignment
                                    {
                                        UserId = user.Id,
                                        PermissionId = permId
                                    });
                                }
                            }
                        }
                    }
                }
            }

            // 6️⃣ Save changes
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new SuccessResponse<Guid>(Guid.Parse(user.Id), "User updated successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new ApplicationException($"Failed to update user: {ex.Message}", ex);
        }
    }
}

