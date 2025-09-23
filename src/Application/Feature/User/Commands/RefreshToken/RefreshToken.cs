using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.RefreshToken;
public record RefreshTokenCommand(string AccessToken)
    : IRequest<SuccessResponse<MobileAuthenticationDto>>;
public class RefreshTokenHandler
    : IRequestHandler<RefreshTokenCommand, SuccessResponse<MobileAuthenticationDto>>
{
    private readonly IAuthenticationService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshTokenHandler(IAuthenticationService authService, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    public async Task<SuccessResponse<MobileAuthenticationDto>> Handle(
        RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = await _authService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            throw new UnAuthorizedException("Invalid access token");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
            throw new UnAuthorizedException("User not found");

        // ✅ Generate new token using existing method
        string newAccessToken = await _authService.GenerateToken(user);

        var dto = new MobileAuthenticationDto
        {
            MobileNumber = user.RegisteredMobileNo ?? string.Empty,
            Name = user.Name,
            AccessToken = newAccessToken,
            isOtpRequired = false,
            ResponseMessage = "Token refreshed successfully"
        };

        return new SuccessResponse<MobileAuthenticationDto>(dto, "Refreshed", "New access token issued.");
    }
}



