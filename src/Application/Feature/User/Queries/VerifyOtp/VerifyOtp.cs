using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.VerifyOtp;
public record VerifyOtpCommand : IRequest<SuccessResponse<OtpAuthenticationDto>>
{
    public string CNIC { get; init; } = default!;
    public string OtpCode { get; init; } = default!;
}
public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, SuccessResponse<OtpAuthenticationDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly IApplicationDbContext _context;
    public VerifyOtpCommandHandler(IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IAuthenticationService authenticationService)
    {
        _context = context;
        _userManager = userManager;
        _authenticationService = authenticationService;
    }
    public async Task<SuccessResponse<OtpAuthenticationDto>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
       
        // Get latest unverified OTP
        var otp = await _context.UserOtps
            .Where(x => x.CNIC == request.CNIC &&
                        !x.IsVerified)
            .OrderByDescending(x => x.Created)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp == null)
            throw new NotFoundException("OTP not found for user.");

        if (otp.ExpiresAt < DateTime.Now)
            throw new ConflictException("OTP has expired.");

        if (otp.OtpCode != request.OtpCode)
            throw new ConflictException("Invalid OTP.");

        // Mark OTP as verified
        otp.IsVerified = true;
        await _context.SaveChangesAsync(cancellationToken);

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == otp.UserId.ToString(), cancellationToken);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.PhoneNumberConfirmed == false)
        {
            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
        }
        if (user.PasswordHash==null || String.IsNullOrEmpty(user.PasswordHash))
        {
            TimeSpan expiresIn = TimeSpan.FromMinutes(5); // temporary token valid for 5 mins
            string temptoken = await _authenticationService.GenerateTemporaryToken(user, "set_password", expiresIn);

            var dto = new OtpAuthenticationDto
            {
                MobileNumber = user.RegisteredMobileNo?.ToString() ?? string.Empty,
                Name = user.Name,
                AccessToken = temptoken,
                isOtpVerified = true,
                Type = "Signup",
                ResponseMessage = "Your mobile number has been verified via OTP.",
                UserType = user.UserType.ToString(),
            };

            return new SuccessResponse<OtpAuthenticationDto>(
                        dto,
                     "Authentication step completed.",
                     "Mobile Number Verified"
                    );
        }

        string token = await _authenticationService.GenerateToken(user);
        IList<string> roles = await _userManager.GetRolesAsync(user);
        string roleName = roles.FirstOrDefault() ?? "NoRoleAssigned";

        var finaldto= new OtpAuthenticationDto
        {
            MobileNumber = user.RegisteredMobileNo?.ToString() ?? string.Empty,
            Name = user.Name,
            AccessToken = token,
            isOtpVerified = true,
            Type = "Login",
            ResponseMessage = "Login successfull",
            UserType = user.UserType.ToString(),
        };

        return new SuccessResponse<OtpAuthenticationDto>(
                       finaldto,
                    "Authentication step completed.",
                    "Mobile Number Verified"
                   );
    }
}

