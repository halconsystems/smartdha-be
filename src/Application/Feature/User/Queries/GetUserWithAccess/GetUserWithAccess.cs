using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserWithAccess;
public record GetUserWithAccessQuery(string ClickedUserId)
    : IRequest<SuccessResponse<UserAccessWithInfoDto>>;
public class GetUserWithAccessQueryHandler
    : IRequestHandler<GetUserWithAccessQuery, SuccessResponse<UserAccessWithInfoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public GetUserWithAccessQueryHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUser)
    {
        _context = context;
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<UserAccessWithInfoDto>> Handle(GetUserWithAccessQuery request, CancellationToken ct)
    {
        // 1️⃣ Load clicked user
        var clickedUser = await _userManager.FindByIdAsync(request.ClickedUserId.ToString());
        if (clickedUser == null)
            throw new NotFoundException(nameof(ApplicationUser), request.ClickedUserId.ToString());

        // 2️⃣ Load clicked user's role
        var clickedUserRole = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == clickedUser.Id)
            .Select(ur => ur.Role.Name)
            .FirstOrDefaultAsync(ct);
        var clickedUserRoleId = await _context.AppUserRoles
           .Include(ur => ur.Role)
           .Where(ur => ur.UserId == clickedUser.Id)
           .Select(ur => ur.Role.Id)
           .FirstOrDefaultAsync(ct);

        // 3️⃣ Load assignments of clicked user
        var clickedUserModuleIds = await _context.UserModuleAssignments
            .Where(x => x.UserId == clickedUser.Id)
            .Select(x => x.ModuleId)
            .ToListAsync(ct);

        var clickedUserSubModuleIds = await _context.UserSubModuleAssignments
            .Where(x => x.UserId == clickedUser.Id)
            .Select(x => x.SubModuleId)
            .ToListAsync(ct);

        var clickedUserPermissionIds = await _context.UserPermissionAssignments
            .Where(x => x.UserId == clickedUser.Id)
            .Select(x => x.PermissionId)
            .ToListAsync(ct);

        // 4️⃣ Get logged-in user info (to know which modules he can assign)
        var loggedInUserId = _currentUser.UserId.ToString();
        var loggedInUserRoles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == loggedInUserId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        var ClickedUser = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == request.ClickedUserId.ToString())
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);



        bool isSuperAdmin = loggedInUserRoles.Contains("SuperAdministrator");
        bool isClickedUserIsSuperAdmin = ClickedUser.Contains("SuperAdministrator");

        List<Module> availableModules;
        //List<ClubsDto> assignedClubs;

        if (isSuperAdmin)
        {
            // super admin → can see everything
            availableModules = await _context.Modules
                .Where(m => m.AppType == AppType.Web)
                .Include(m => m.SubModules)
                    .ThenInclude(sm => sm.Permissions)
                .ToListAsync(ct);
        }
        else
        {
            // normal admin → only what he has access to
            var myModuleIds = await _context.UserModuleAssignments
                .Where(x => x.UserId == loggedInUserId)
                .Select(x => x.ModuleId)
                .ToListAsync(ct);

            var myPermissionIds = await _context.UserPermissionAssignments
                .Where(x => x.UserId == loggedInUserId)
                .Select(x => x.PermissionId)
                .ToListAsync(ct);

            availableModules = await _context.Modules
                .Where(m => myModuleIds.Contains(m.Id) && m.AppType == AppType.Web)
                .Include(m => m.SubModules)
                    .ThenInclude(sm => sm.Permissions)
                .ToListAsync(ct);

            // filter perms by my rights
            foreach (var module in availableModules)
            {
                foreach (var sub in module.SubModules)
                {
                    sub.Permissions = sub.Permissions
                        .Where(p => myPermissionIds.Contains(p.Id))
                        .ToList();
                }
                module.SubModules = module.SubModules.Where(sm => sm.Permissions.Any()).ToList();
            }


            // ✅ Only assigned clubs
           


        }

        //if (isClickedUserIsSuperAdmin)
        //{
        //    // ✅ All clubs
        //    assignedClubs = await _olmrcontext.Clubs
        //       .Select(c => new ClubsDto
        //       {
        //           ClubId = c.Id,
        //           ClubName = c.Name
        //       })
        //       .ToListAsync(ct);
        //}
        //else
        //{
        //    // 1. Get assigned ClubIds for this user
        //    var assignedClubIds = await _context.UserClubAssignments
        //        .Where(uca => uca.UserId == clickedUser.Id)
        //        .Select(uca => uca.ClubId)
        //        .ToListAsync(ct);

        //    // 2. Query Clubs from the other DbContext
        //    assignedClubs = await _olmrcontext.Clubs
        //       .Where(c => assignedClubIds.Contains(c.Id))
        //       .Select(c => new ClubsDto
        //       {
        //           ClubId = c.Id,
        //           ClubName = c.Name
        //       })
        //       .ToListAsync(ct);
        //}

        // 5️⃣ Build tree: mark AlreadyAccess = true if clicked user already has it
        var moduleTree = availableModules.Select(m => new ModuleTreeDto
        {
            Id = m.Id,
            Name = m.DisplayName,
            Value = m.Value,
            DisplayName = m.DisplayName,
            AlreadyAccess = clickedUserModuleIds.Contains(m.Id),
            SubModules = m.SubModules.Select(sm => new SubModuleTreeDto
            {
                Id = sm.Id,
                Name = sm.DisplayName,
                Value = sm.Value,
                DisplayName = sm.DisplayName,
                AlreadyAccess = clickedUserSubModuleIds.Contains(sm.Id),
                Permissions = sm.Permissions.Select(p => new PermissionTreeDto
                {
                    Id = p.Id,
                    Name = p.DisplayName,
                    Value = p.Value,
                    DisplayName = p.DisplayName,
                    AlreadyAccess = clickedUserPermissionIds.Contains(p.Id)
                }).ToList()
            }).ToList()
        }).ToList();



        // 6️⃣ Response
        var result = new UserAccessWithInfoDto
        {
            Id = Guid.Parse(clickedUser.Id),
            Name = clickedUser.Name,
            Email = clickedUser.Email ?? string.Empty,
            MobileNo = clickedUser.MobileNo ?? string.Empty,
            CNIC = clickedUser.CNIC ?? string.Empty,
            UserName=clickedUser.UserName ?? string.Empty,
            Role = clickedUserRole ?? "User",
            RoleId = clickedUserRoleId.ToString(),
            Modules = moduleTree,
           // Clubs= assignedClubs
        };

        return new SuccessResponse<UserAccessWithInfoDto>(result);
    }
}

