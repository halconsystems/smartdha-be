using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.SetPassword;
public record SetPasswordCommand : IRequest<SuccessResponse<string>>
{
    public string CNIC { get; init; } = default!;
    public string Password { get; init; } = default!;
}

public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, SuccessResponse<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public SetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<SuccessResponse<string>> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.CNIC == request.CNIC, cancellationToken);

        if (user == null)
            throw new NotFoundException("User not found with the provided CNIC.");

        var hasPassword = await _userManager.HasPasswordAsync(user);

        IdentityResult result;
        if (hasPassword)
        {
            // Remove old password and set new one
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            result = await _userManager.ResetPasswordAsync(user, resetToken, request.Password);
        }
        else
        {
            // Just add the new password
            result = await _userManager.AddPasswordAsync(user, request.Password);
        }

        if (!result.Succeeded)
        {
            var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
            throw new ConflictException($"Password update failed: {errors}");
        }

        string finalmsg= "Your password was updated successfully. Please keep it safe.";
        return SuccessResponse<string>.FromMessage(finalmsg);
    }
}

