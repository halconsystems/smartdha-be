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
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.ForgetPassword.Command;

using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

public record OTPSendCommand(string Cnic) : IRequest<SuccessResponse<MobileAuthenticationDto>>;

public class OTPSendCommandHandler : IRequestHandler<OTPSendCommand, SuccessResponse<MobileAuthenticationDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;
    private readonly ISmsService _otpService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IActivityLogger _activityLogger;


    public OTPSendCommandHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context,
        ISmsService otpService,
        ICurrentUserService currentUserService,
        IAuthenticationService service,
        IActivityLogger activityLogger)
    {
        _userManager = userManager;
        _context = context;
        _otpService = otpService;
        _currentUserService = currentUserService;
        _authenticationService = service;
        _activityLogger = activityLogger;
    }

    public async Task<SuccessResponse<MobileAuthenticationDto>> Handle(OTPSendCommand request, CancellationToken cancellationToken)
    {

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.CNIC == request.Cnic);

        if (user == null)
        {
            await _activityLogger.LogAsync("LoginFailed", email: request.Cnic, description: "Invalid Email", appType: AppType.Mobile);
            throw new UnAuthorizedException("Invalid CNIC. Please verify and try again.");

        }
        else if (user.AppType != AppType.Mobile)
        {
            await _activityLogger.LogAsync("InvalidApp", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "User not authorized for this portal.", appType: AppType.Mobile);
            throw new UnAuthorizedException("You are not authorized to access this application.");
        }
        else if (user.IsActive == false)
        {
            await _activityLogger.LogAsync("InactiveUserLogin", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "Inactive user tried to login", appType: AppType.Mobile);
            throw new UnAuthorizedException("Your account is inactive. Please contact the administrator for assistance.");
        }
        else if (user.IsDeleted == true)
        {
            await _activityLogger.LogAsync("DeletedUserLogin", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "User is deleted. Contact administrator.", appType: AppType.Mobile);
            throw new UnAuthorizedException("Your account has been deleted. Please contact the administrator for assistance.");
        }
        else if (user.PhoneNumberConfirmed == false)
        {
            await _activityLogger.LogAsync("NotVerifiedNumber", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "Your mobile number is not verified.", appType: AppType.Mobile);
            throw new UnAuthorizedException("Your mobile number is not verified. Please verify to continue.");
        }

        // Normalize mobile number
        string normalizedMobile = (user.RegisteredMobileNo ?? string.Empty)
                .Replace("-", "")              // remove dashes
                .Replace(" ", "")              // remove spaces
                .Trim();                       // remove leading/trailing whitespace

        if (normalizedMobile.StartsWith("0"))
        {
            normalizedMobile = "92" + normalizedMobile.Substring(1);
        }

        // Generate OTP
        var otp = new Random().Next(100000, 999999);
        string sentMessage = $"{otp} is your OTP to DHA Karachi mobile application. Do not share it with anyone.";
        string returnMessage = $"An OTP has been sent to the mobile number registered with your membership/staff/club-membership";

        // Send SMS
        string result = await _otpService.SendSmsAsync(normalizedMobile, sentMessage, cancellationToken);
        if (!result.Equals("SUCCESSFUL", StringComparison.OrdinalIgnoreCase))
            throw new Exception("Failed to send OTP. Please try again later.");

        // Save OTP
        var userOtp = new UserOtp
        {
            UserId = Guid.Parse(user.Id),
            CNIC = user.CNIC,
            MobileNo = normalizedMobile,
            OtpCode = otp.ToString(),
            SentMessage = sentMessage,
            IsVerified = false,
            ExpiresAt = DateTime.Now.AddMinutes(2)
        };

        TimeSpan expiresIn = TimeSpan.FromMinutes(2); // 2-minute validity
        string verifyToken = await _authenticationService.GenerateTemporaryToken(user, "verify_otp", expiresIn);


        _context.UserOtps.Add(userOtp);
        await _context.SaveChangesAsync(cancellationToken);

        var finaldto = new MobileAuthenticationDto
        {
            MobileNumber = MaskMobile(user.RegisteredMobileNo?.ToString()) ?? string.Empty,
            Name = "",
            AccessToken = verifyToken,
            isOtpRequired = true,
            ResponseMessage = returnMessage
        };
        await _activityLogger.LogAsync("OTP Verification", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "OTP Verification Required", appType: AppType.Mobile);

        return new SuccessResponse<MobileAuthenticationDto>(
                finaldto,
             "Authentication step completed.",
             "OTP Verification Required"
            );
    }

    public static string MaskMobile(string? normalizedMobile)
    {
        if (string.IsNullOrEmpty(normalizedMobile))
            return string.Empty;

        // Keep only last 4 digits
        string lastFour = normalizedMobile.Length > 4
            ? normalizedMobile[^4..]
            : normalizedMobile;

        // Replace the rest with asterisks
        string masked = new string('*', normalizedMobile.Length - lastFour.Length) + lastFour;

        return masked;
    }
}


