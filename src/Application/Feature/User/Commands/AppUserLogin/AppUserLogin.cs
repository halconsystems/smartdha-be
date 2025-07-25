using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.Login;
public record AppUserLoginCommand : IRequest<AuthenticationDto>
{
    public string CNIC { get; set; } = default!;
    public string Password { get; set; } = default!;
}
public class AppUserLoginHandler : IRequestHandler<AppUserLoginCommand, AuthenticationDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AppUserLoginHandler(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationService authenticationService,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authenticationService = authenticationService;
        _roleManager = roleManager;
    }
    public async Task<AuthenticationDto> Handle(AppUserLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.CNIC == request.CNIC);

        if (user == null)
        {
            throw new UnAuthorizedException("Invalid CNIC");
        }
        else if (user.AppType != AppType.Mobile)
        {
            throw new UnAuthorizedException("User not authorized for this application.");
        }
        else if (user.IsActive == false)
        {
            throw new UnAuthorizedException("User marked InActive contact with administrator");
        }
        else if (user.IsDeleted == true)
        {
            throw new UnAuthorizedException("User is deleted contact with administrator");
        }

        SignInResult result = await _signInManager.PasswordSignInAsync(request.CNIC, request.Password, false, lockoutOnFailure: false);

        if (!result.Succeeded && !result.RequiresTwoFactor)
        {
            throw new UnAuthorizedException("Invalid Password");
        }
        string token = await _authenticationService.GenerateToken(user);

        IList<string> roles = await _userManager.GetRolesAsync(user);

        return new AuthenticationDto
        {
            AccessToken = token,
            Role = roles.FirstOrDefault()!,
            ResponseMessage = "Authenticated!"
        };
    }
}
