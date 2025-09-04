using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.SubModules.Queries.SubModuleList;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
public record GenerateTokenCommand : IRequest<AuthenticationDto>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
public class GenerateTokenHandler : IRequestHandler<GenerateTokenCommand, AuthenticationDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly IApplicationDbContext _context;
    private readonly IPermissionCache _permissionCache;   // 🔹 Injected Redis cache
    private readonly ILogger<GenerateTokenCommand> _logger;
    private readonly IActivityLogger _activityLogger;

    public GenerateTokenHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationService authenticationService,
        IApplicationDbContext context,
        IPermissionCache permissionCache,
        ILogger<GenerateTokenCommand> logger,
        IActivityLogger activityLogger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authenticationService = authenticationService;
        _context = context;
        _permissionCache = permissionCache;
        _logger = logger;
        _activityLogger = activityLogger;
    }

    public async Task<AuthenticationDto> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
    {
        string username = "";
        // 1️⃣ Validate user
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            await _activityLogger.LogAsync("LoginFailed", email: request.Email, description: "Invalid Email", appType: AppType.Web);
            throw new UnAuthorizedException("Invalid Email");
        }
        else
        {
            username = user.UserName ?? user.Email ?? "";
        }

        if (user.AppType != AppType.Web)
        {
            await _activityLogger.LogAsync("InvalidApp", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "User not authorized for this portal.", appType: AppType.Web);
            throw new UnAuthorizedException("User not authorized for this portal.");
        }

        if (!user.IsActive)
        {
            await _activityLogger.LogAsync("InactiveUserLogin", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "Inactive user tried to login", appType: AppType.Web);
            throw new UnAuthorizedException("User is marked InActive. Contact administrator.");
        }

        if (user.IsDeleted == true)
        {
            await _activityLogger.LogAsync("DeletedUserLogin", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "User is deleted. Contact administrator.", appType: AppType.Web);
            throw new UnAuthorizedException("User is deleted. Contact administrator.");
        }

        // 2️⃣ Validate password
        var result = await _signInManager.PasswordSignInAsync(username, request.Password, false, lockoutOnFailure: false);
        if (!result.Succeeded && !result.RequiresTwoFactor)
        {
            await _activityLogger.LogAsync("LoginFailed", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "Invalid Password", appType: AppType.Web);
            throw new UnAuthorizedException("Invalid Password");
        }

        // 3️⃣ Generate JWT
        string token = await _authenticationService.GenerateWebUserToken(user);

        // 4️⃣ Check roles
        var roles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        List<Guid> userPermissionIds = new();
        List<Guid> userSubModuleIds = new(); // ✅ declare here so it’s available later
        List<Module> modules;

        if (isSuperAdmin)
        {
            // ✅ SuperAdmin → all modules + submodules + permissions
            modules = await _context.Modules
                .Where(m => m.AppType == AppType.Web)
                .Include(m => m.SubModules)
                    .ThenInclude(sm => sm.Permissions)
                .ToListAsync(cancellationToken);

            userPermissionIds = modules.SelectMany(m => m.SubModules)
                                       .SelectMany(sm => sm.Permissions)
                                       .Select(p => p.Id)
                                       .ToList();

            // for consistency, give superadmin all submodules
            userSubModuleIds = modules.SelectMany(m => m.SubModules)
                                      .Select(sm => sm.Id)
                                      .ToList();
        }
        else
        {
            // ✅ Normal user → only assigned modules + submodules + permissions
            var userModuleIds = await _context.UserModuleAssignments
                .Where(x => x.UserId == user.Id)
                .Select(x => x.ModuleId)
                .ToListAsync(cancellationToken);

            userSubModuleIds = await _context.UserSubModuleAssignments
                .Where(x => x.UserId == user.Id)
                .Select(x => x.SubModuleId)
                .ToListAsync(cancellationToken);

            userPermissionIds = await _context.UserPermissionAssignments
                .Where(up => up.UserId == user.Id)
                .Select(up => up.PermissionId)
                .ToListAsync(cancellationToken);

            modules = await _context.Modules
                .Where(m => userModuleIds.Contains(m.Id) && m.AppType == AppType.Web)
                .Include(m => m.SubModules)
                    .ThenInclude(sm => sm.Permissions)
                .ToListAsync(cancellationToken);
        }

        // 5️⃣ Build DTO tree
        var moduleresult = modules.Select(m => new ModuleTreeDto
        {
            Id = m.Id,
            Name = m.Name ?? m.DisplayName,
            Value = m.Value,
            DisplayName = m.DisplayName,

            SubModules = m.SubModules
                .Where(sm => isSuperAdmin || userSubModuleIds.Contains(sm.Id)) // ✅ only assigned submodules
                .Select(sm => new SubModuleTreeDto
                {
                    Id = sm.Id,
                    Name = sm.Name ?? sm.DisplayName,
                    Value = sm.Value,
                    DisplayName = sm.DisplayName,

                    Permissions = sm.Permissions
                        .Where(p => isSuperAdmin || userPermissionIds.Contains(p.Id)) // ✅ only assigned perms
                        .Select(p => new PermissionTreeDto
                        {
                            Id = p.Id,
                            Name = p.DisplayName,
                            Value = p.Value,
                            DisplayName = p.DisplayName
                        }).ToList()
                }).ToList()
        }).ToList();


        // 6️⃣ Flatten for Redis
        var flatPermissions = new List<string>();
        foreach (var module in modules)
        {
            flatPermissions.Add(module.Value); // module-level

            foreach (var sm in module.SubModules)
            {
                flatPermissions.Add(sm.Value); // submodule-level

                foreach (var perm in sm.Permissions)
                {
                    flatPermissions.Add($"{sm.Value}.{perm.Value}"); // permission-level
                }
            }
        }

        try
        {
            await _permissionCache.SetPermissionsAsync(user.Id, flatPermissions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Redis unavailable for user {user.Id}, skipping cache.");
        }

        await _activityLogger.LogAsync("LoginSuccess", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "User logged in successfully", appType: AppType.Web);

        // 7️⃣ Return DTO
        return new AuthenticationDto
        {
            AccessToken = token,
            Role = roles.FirstOrDefault() ?? "User",
            Modules = moduleresult,
            Name = user.Name,
            Email = user.Email ?? string.Empty,
            ResponseMessage = "Authenticated!"
        };
    }


}
