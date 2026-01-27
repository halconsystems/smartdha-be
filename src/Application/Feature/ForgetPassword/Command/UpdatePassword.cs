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
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.ForgetPassword.Command;

public record ForgetPasswordCommand(
    string Cnic,
    string ResetToken,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<SuccessResponse<string>>;

public class ForgetPasswordCommandHandler :IRequestHandler<ForgetPasswordCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IActivityLogger _activityLogger;
    private readonly IAuthenticationService _authenticationService;

    public ForgetPasswordCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IActivityLogger activityLogger,IAuthenticationService service)
    {
        _context = context;
        _userManager = userManager;
        _activityLogger = activityLogger;
        _authenticationService = service;
    }

    public async Task<SuccessResponse<string>> Handle(ForgetPasswordCommand command,CancellationToken ct)
    {
        var userDetails =  await _userManager.Users.FirstOrDefaultAsync(x => x.CNIC == command.Cnic,ct);

        if(userDetails == null)
            throw new KeyNotFoundException("User not found.");


        var principal = _authenticationService.ValidateTemporaryToken(
            command.ResetToken,
            expectedPurpose: "reset_password");

        string userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnAuthorizedException("Invalid reset token.");

        IdentityResult result;
        if (command.NewPassword == command.ConfirmNewPassword)
        {
            var hasPassword = await _userManager.HasPasswordAsync(userDetails);

            
            if (hasPassword)
            {
                // Remove old password and set new one
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userDetails);
                result = await _userManager.ResetPasswordAsync(userDetails, resetToken, command.NewPassword);
                
            }
            else
            {
                // Just add the new password
                result = await _userManager.AddPasswordAsync(userDetails, command.NewPassword);
                
            }
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                await _activityLogger.LogAsync("ForgetPassword", email: command.Cnic, description: result.Errors.Select(e => e.Description).ToString(), appType: AppType.Mobile);
                throw new ConflictException($"Password update failed: {errors}");
            }
            string finalmsg = "Your password was updated successfully. Please keep it safe.";
            await _activityLogger.LogAsync("ForgetPassword", email: command.Cnic, description: "Your password was updated successfully. Please keep it safe.", appType: AppType.Mobile);
            return SuccessResponse<string>.FromMessage(finalmsg);
        }
        else
        {
            string finalmsg = "Your New Password And Confirm Password is not Matched.";
            await _activityLogger.LogAsync("ForgetPassword", email: command.Cnic, description: finalmsg, appType: AppType.Mobile);
            return SuccessResponse<string>.FromMessage(finalmsg);
        }

    }
}
