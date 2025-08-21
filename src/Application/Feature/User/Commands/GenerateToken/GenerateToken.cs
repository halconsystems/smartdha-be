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

    public GenerateTokenHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationService authenticationService,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authenticationService = authenticationService;
        _context = context;
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

        var userRoles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync();



        //IList<string> roles = await _userManager.GetRolesAsync(user);
        string userRole = userRoles.FirstOrDefault() ?? "User";

        // -------- Fetch Modules Assigned to User --------
        var userModuleIds = await _context.UserModuleAssignments
            .Where(x => x.UserId == user.Id)
            .Select(x => x.ModuleId)
            .ToListAsync(cancellationToken);

        // ✅ Get assigned modules including submodules + permissions
        var modules = await _context.Modules
            .Where(m => userModuleIds.Contains(m.Id) && m.AppType == AppType.Web)
            .Include(m => m.SubModules)
                .ThenInclude(sm => sm.Permissions)
            .ToListAsync(cancellationToken);

        // ✅ Get user permissions (explicitly stored)
        var userPermissions = await _context.UserPermissions
            .Where(up => up.UserId == user.Id)
            .ToListAsync(cancellationToken);

        var moduleDtos = modules.Select(module => new ModuleDto
        {
            ModuleId = module.Id,
            ModuleName = module.Name,
            ModuleURL = module.URL,
            DisplayName= module.DisplayName,
            Value= module.Value,

            SubModules = module.SubModules.Select(sm => new SubModuleDto
            {
                SubModuleId = sm.Id,
                SubModuleName = sm.Name,
                DisplayName = sm.DisplayName,
                Value = sm.Value,

                Permissions = sm.Permissions.Select(p => new AllPermissionDto
                {
                    PermissionId = p.Id,
                    Value = p.Value,
                    DisplayName = p.DisplayName
                }).ToList()
            }).ToList()
        }).ToList();


        return new AuthenticationDto
        {
            AccessToken = token,
            Role = userRole,
            Modules = moduleDtos,
            Name=user.Name,
            Email = user.Email?.ToString() ?? string.Empty,
            ResponseMessage = "Authenticated!"
        };
    }
}
