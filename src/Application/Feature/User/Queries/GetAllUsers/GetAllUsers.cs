using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllUsers;
public record GetAllUsersQuery : IRequest<SuccessResponse<List<UserListDto>>>;

public class GetAllUsersHandler
    : IRequestHandler<GetAllUsersQuery, SuccessResponse<List<UserListDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllUsersHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<List<UserListDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(currentUserId))
            throw new UnAuthorizedException("Invalid user context.");

        // 1️⃣ Check if current user is SuperAdmin
        var currentRoles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == currentUserId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        bool isSuperAdmin = currentRoles.Contains("SuperAdministrator");

        // 2️⃣ Get base users
        var usersQuery = _userManager.Users.AsNoTracking().AsQueryable();

        if (!isSuperAdmin)
        {

            List<string> accessibleUserIds = new();

            // 3️⃣ Normal user → filter to users assigned to the same clubs
            var myClubIds = await _context.UserClubAssignments
                .Where(uca => uca.UserId == currentUserId)
                .Select(uca => uca.ClubId)
                .ToListAsync(cancellationToken);

            if (myClubIds.Any())
            {
                accessibleUserIds = await _context.UserClubAssignments
                .Where(uca => myClubIds.Contains(uca.ClubId))
                .Select(uca => uca.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);
            }
            else
            {
                // --- ✅ Module-based filtering (excluding "UserManagement") ---
                var myModuleIds = await _context.UserModuleAssignments
                    .Include(uma => uma.Module)
                    .Where(uma => uma.UserId == currentUserId && uma.Module.Value != "UserManagement")
                    .Select(uma => uma.ModuleId)
                    .ToListAsync(cancellationToken);

                if (myModuleIds.Any())
                {
                    accessibleUserIds = await _context.UserModuleAssignments
                        .Include(uma => uma.Module)
                        .Where(uma => myModuleIds.Contains(uma.ModuleId) &&
                                      uma.Module.Value.ToLower() != "usermanagement")
                        .Select(uma => uma.UserId)
                        .Distinct()
                        .ToListAsync(cancellationToken);
                }
            }

            if (accessibleUserIds.Any())
                usersQuery = usersQuery.Where(u => accessibleUserIds.Contains(u.Id));
            else
                usersQuery = usersQuery.Where(u => false); // no access
        }

        var users = await usersQuery.ToListAsync(cancellationToken);

        var result = new List<UserListDto>();

        foreach (var user in users)
        {
            // Fetch role(s)
            var roleNames = await _context.AppUserRoles
                .Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.Name)
                .ToListAsync(cancellationToken);

            result.Add(new UserListDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                MobileNo = user.MobileNo,
                CNIC = user.CNIC,
                AppType = user.AppType.ToString(),
                UserType = user.UserType.ToString(),
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                Role = roleNames.Any() ? string.Join(", ", roleNames) : "-"
            });
        }

        return new SuccessResponse<List<UserListDto>>(result);
    }
}

