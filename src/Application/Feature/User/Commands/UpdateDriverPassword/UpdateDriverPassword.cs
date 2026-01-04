using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.UpdateDriverPassword;
public record UpdateDriverPasswordCommand(
    Guid DriverId,
    string NewPassword
) : IRequest<Unit>;
public class UpdateDriverPasswordCommandHandler
    : IRequestHandler<UpdateDriverPasswordCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateDriverPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(UpdateDriverPasswordCommand request, CancellationToken ct)
    {
        var driver = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.DriverId.ToString()
                && x.UserType == UserType.Driver
                && !x.IsDeleted, ct)
            ?? throw new NotFoundException("Driver not found.");

        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(driver);

        var result = await _userManager.ResetPasswordAsync(driver, resetToken, request.NewPassword);
        if (!result.Succeeded)
        {
            string err = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new Exception("Password update failed: " + err);
        }

        return Unit.Value;
    }
}

