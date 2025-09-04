using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.UpdatePassword;
public record UpdatePasswordCommand(string CurrentPassword, string NewPassword)
    : IRequest<SuccessResponse<string>>;
public class UpdatePasswordCommandHandler
    : IRequestHandler<UpdatePasswordCommand, SuccessResponse<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public UpdatePasswordCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<string>> Handle(UpdatePasswordCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());
        if (user == null)
            throw new UnAuthorizedException("User not found");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"Password update failed: {errors}");
        }

        return new SuccessResponse<string>("Password updated successfully.");
    }
}

