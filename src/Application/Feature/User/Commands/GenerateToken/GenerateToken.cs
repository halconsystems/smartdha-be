using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.SubModules.Queries.SubModuleList;
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

    public GenerateTokenHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationService authenticationService,
        IApplicationDbContext context,
        IPermissionCache permissionCache,
        ILogger<GenerateTokenCommand> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authenticationService = authenticationService;
        _context = context;
        _permissionCache = permissionCache;
        _logger=logger;
    }

    public async Task<AuthenticationDto> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new UnAuthorizedException("Invalid Email");

        if (user.AppType != AppType.Web)
            throw new UnAuthorizedException("User not authorized for this portal.");

        if (!user.IsActive)
            throw new UnAuthorizedException("User is marked InActive. Contact administrator.");

        if (user.IsDeleted == true)
            throw new UnAuthorizedException("User is deleted. Contact administrator.");

        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, lockoutOnFailure: false);

        if (!result.Succeeded && !result.RequiresTwoFactor)
            throw new UnAuthorizedException("Invalid Password");

        string token = await _authenticationService.GenerateWebUserToken(user);

        // -------- Fetch Modules Assigned to User --------
        // ✅ Get assigned modules
        var userModuleIds = await _context.UserModuleAssignments
            .Where(x => x.UserId == user.Id)
            .Select(x => x.ModuleId)
            .ToListAsync(cancellationToken);

        var modules = await _context.Modules
            .Where(m => userModuleIds.Contains(m.Id) && m.AppType == AppType.Web)
            .Include(m => m.SubModules)
                .ThenInclude(sm => sm.Permissions)
            .ToListAsync(cancellationToken);

        var userPermissions = await _context.UserPermissions
            .Where(up => up.UserId == user.Id)
            .ToListAsync(cancellationToken);

        // ✅ Flatten permissions into strings
        var flatPermissions = new List<string>();
        foreach (var module in modules)
        {
            foreach (var sm in module.SubModules)
            {
                if (!sm.RequiresPermission)
                {
                    // SubModule-level access
                    flatPermissions.Add(sm.Value);
                }
                else
                {
                    var userPerm = userPermissions.FirstOrDefault(up => up.SubModuleId == sm.Id);
                    if (userPerm != null)
                    {
                        var actions = userPerm.AllowedActions.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var act in actions)
                        {
                            flatPermissions.Add($"{sm.Value}.{act.Trim()}");
                        }
                    }
                }
            }
        }

        // ✅ Save to Redis (with graceful failure handling)
        try
        {
            await _permissionCache.SetPermissionsAsync(user.Id, flatPermissions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Redis unavailable for user {user.Id}, skipping cache.");
        }

        // ✅ Return DTO
        return new AuthenticationDto
        {
            AccessToken = token,
            Role = (await _context.AppUserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)
                .FirstOrDefaultAsync()) ?? "User",
            Modules = modules.Select(m => new ModuleDto
            {
                ModuleId = m.Id,
                DisplayName = m.DisplayName,
                Value = m.Value,
                SubModules = m.SubModules.Select(sm => new SubModuleDto
                {
                    SubModuleId = sm.Id,
                    DisplayName = sm.DisplayName,
                    Value = sm.Value,
                    Permissions = sm.Permissions.Select(p => new AllPermissionDto
                    {
                        PermissionId = p.Id,
                        Value = p.Value,
                        DisplayName = p.DisplayName
                    }).ToList()
                }).ToList()
            }).ToList(),
            Name = user.Name,
            Email = user.Email ?? string.Empty,
            ResponseMessage = "Authenticated!"
        };
    }
}
