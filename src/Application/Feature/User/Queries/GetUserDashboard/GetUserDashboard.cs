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
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserDashboard;
public record GetUserDashboardQuery : IRequest<SuccessResponse<UserDashboardDto>>;

public class GetUserDashboardQueryHandler
    : IRequestHandler<GetUserDashboardQuery, SuccessResponse<UserDashboardDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    public GetUserDashboardQueryHandler(IApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<UserDashboardDto>> Handle(GetUserDashboardQuery request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(currentUserId))
            throw new UnAuthorizedException("Invalid user context.");

        // 1️⃣ Check if current user is SuperAdmin
        var currentRoles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == currentUserId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = currentRoles.Contains("SuperAdministrator");

        // 2️⃣ Get base users
        var usersQuery = _userManager.Users.AsNoTracking().AsQueryable();
        List<string> visibleUserIds;

        if (!isSuperAdmin)
        {
            // 3️⃣ Normal user → filter users assigned to same clubs
            var myClubIds = await _context.UserClubAssignments
                .Where(uca => uca.UserId == currentUserId)
                .Select(uca => uca.ClubId)
                .ToListAsync(ct);

            visibleUserIds = await _context.UserClubAssignments
                .Where(uca => myClubIds.Contains(uca.ClubId))
                .Select(uca => uca.UserId)
                .Distinct()
                .ToListAsync(ct);

            usersQuery = usersQuery.Where(u => visibleUserIds.Contains(u.Id));
        }
        else
        {
            visibleUserIds = await usersQuery.Select(u => u.Id).ToListAsync(ct);
        }

        var users = await usersQuery.ToListAsync(ct);

        // 4️⃣ Always calculate these
        var totalUsers = users.Count();
        var activeUsers = users.Count(u => u.IsActive && !u.IsDeleted);
        var inactiveUsers = users.Count(u => !u.IsActive && !u.IsDeleted);
        var deletedUsers = users.Count(u => u.IsDeleted);

        // 5️⃣ Defaults
        int verifiedUsers = 0, nonVerifiedUsers = 0;
        Dictionary<string, int> userTypeCounts = new();
        Dictionary<string, int> appTypeCounts = new();
        List<DailyUserTrendDto> dailyTrends = new();
        List<RoleUserCountDto> roleWiseUsers = new();

        // 6️⃣ If SuperAdmin → calculate full stats
        if (isSuperAdmin)
        {
            verifiedUsers = users.Count(u => u.IsVerified);
            nonVerifiedUsers = users.Count(u => !u.IsVerified);

            userTypeCounts = users
                .GroupBy(u => u.UserType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            appTypeCounts = users
                .GroupBy(u => u.AppType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var today = DateTime.Now.Date;

            dailyTrends = users
                .Where(u => u.CreatedDateTime >= startOfMonth && u.CreatedDateTime <= today)
                .GroupBy(u => u.CreatedDateTime.Date)
                .Select(g => new DailyUserTrendDto
                {
                    Date = g.Key,
                    RegisteredCount = g.Count(),
                    VerifiedCount = g.Count(x => x.IsVerified),
                    MemberCount = g.Count(x => x.UserType == UserType.Member),
                    NonMemberCount = g.Count(x => x.UserType == UserType.NonMember)
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        // 7️⃣ Role-wise users (always include, but scoped to visibleUserIds)
        roleWiseUsers = await _context.AppRoles
            .Select(r => new RoleUserCountDto
            {
                RoleName = r.Name,
                UserCount = r.UserRoles.Count(ur => visibleUserIds.Contains(ur.UserId) && !ur.User.IsDeleted)
            })
            .Where(r => r.UserCount > 0)
            .OrderByDescending(r => r.UserCount)
            .ToListAsync(ct);

        // 8️⃣ Final DTO
        var dto = new UserDashboardDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = inactiveUsers,
            DeletedUsers = deletedUsers,

            VerifiedUsers = verifiedUsers,
            NonVerifiedUsers = nonVerifiedUsers,
            UserTypeCounts = userTypeCounts,
            AppTypeCounts = appTypeCounts,
            DailyTrends = dailyTrends,
            RoleWiseUsers = roleWiseUsers
        };

        return new SuccessResponse<UserDashboardDto>(dto);
    }
}

