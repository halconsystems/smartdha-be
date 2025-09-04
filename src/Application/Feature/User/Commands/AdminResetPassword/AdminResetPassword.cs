using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.AdminResetPassword;
public record AdminResetPasswordCommand(Guid UserId, string NewPassword)
    : IRequest<SuccessResponse<string>>;


public class AdminResetPasswordCommandHandler
    : IRequestHandler<AdminResetPasswordCommand, SuccessResponse<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<SuccessResponse<string>> Handle(AdminResetPasswordCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            throw new NotFoundException("User not found");

        // generate token internally
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"Admin password reset failed: {errors}");
        }

        return new SuccessResponse<string>("Password reset successfully by Admin.");
    }
}

