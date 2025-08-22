using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserDashboard;
public record GetUserDashboardQuery : IRequest<SuccessResponse<UserDashboardDto>>;

public class GetUserDashboardQueryHandler
    : IRequestHandler<GetUserDashboardQuery, SuccessResponse<UserDashboardDto>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserDashboardQueryHandler(IApplicationDbContext ctx, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _ctx = ctx;
        _roleManager = roleManager;
        _userManager = userManager;

    }

    public async Task<SuccessResponse<UserDashboardDto>> Handle(GetUserDashboardQuery request, CancellationToken ct)
    {
        var users =await _userManager.Users.ToListAsync(ct);   // <-- from your DbContext, not UserManager

        var totalUsers =  users.Count();
        var activeUsers =  users.Count(u => u.IsActive && !u.IsDeleted);
        var inactiveUsers =  users.Count(u => !u.IsActive && !u.IsDeleted);
        var verifiedUsers =  users.Count(u => u.IsVerified);
        var nonVerifiedUsers =  users.Count(u => !u.IsVerified);
        var deletedUsers =  users.Count(u => u.IsDeleted);

        var userTypeCounts = users
            .GroupBy(u => u.UserType)
            .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
            .ToDictionary(x => x.Type, x => x.Count);

        var appTypeCounts = users
            .GroupBy(u => u.AppType)
            .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
            .ToDictionary(x => x.Type, x => x.Count);

        // Current month trend
        var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var today = DateTime.Now.Date;

        var dailyTrends = users
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

        // Role-wise user count
        // Role-wise user count (using AppRole not RoleManager)
        var roleWiseUsers = await _ctx.AppRoles
            .Select(r => new RoleUserCountDto
            {
                RoleName = r.Name,
                UserCount = r.UserRoles.Count(ur => !ur.User.IsDeleted) // only non-deleted users
            })
            .OrderByDescending(r => r.UserCount)
            .ToListAsync(ct);


        var dto = new UserDashboardDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = inactiveUsers,
            VerifiedUsers = verifiedUsers,
            NonVerifiedUsers = nonVerifiedUsers,
            DeletedUsers = deletedUsers,
            UserTypeCounts = userTypeCounts,
            AppTypeCounts = appTypeCounts,
            DailyTrends = dailyTrends,
            RoleWiseUsers = roleWiseUsers
        };

        return new SuccessResponse<UserDashboardDto>(dto);
    }
}

