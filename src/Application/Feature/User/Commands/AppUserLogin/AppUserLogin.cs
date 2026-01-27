using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.Login;
public record   AppUserLoginCommand : IRequest<SuccessResponse<MobileAuthenticationDto>>
{
    public string CNIC { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FCMToken { get; set; } = default!;
    public string DeviceId { get; set; } = default!;
}
public class AppUserLoginHandler : IRequestHandler<AppUserLoginCommand, SuccessResponse<MobileAuthenticationDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly ISmsService _otpService;
    private readonly IApplicationDbContext _context;
    private readonly IActivityLogger _activityLogger;

    public AppUserLoginHandler(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationService authenticationService,
        ISmsService otpService,
        IApplicationDbContext context,
        IActivityLogger activityLogger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authenticationService = authenticationService;
        _otpService = otpService;
        _context = context;
        _activityLogger = activityLogger;
    }
    public async Task<SuccessResponse<MobileAuthenticationDto>> Handle(AppUserLoginCommand request, CancellationToken cancellationToken)
    {

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.CNIC == request.CNIC);

        if (user == null)
        {
            await _activityLogger.LogAsync("LoginFailed", email: request.CNIC, description: "Invalid Email", appType: AppType.Mobile);
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
        else if(user.UserType==UserType.NonMember)
        {
            if (user.IsVerified == false)
            {
                var existingRequest = await _context.NonMemberVerifications
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

                if (existingRequest != null)
                {
                    string responseMessage = existingRequest?.Status switch
                    {
                        VerificationStatus.Pending => "Your registration request is under review.",
                        VerificationStatus.Rejected => "Your registration request was rejected. Please contact the administrator.",
                        VerificationStatus.Approved => "Your account is already registered.",
                        _ => "Your account is already registered."
                    };


                    var newdto = new MobileAuthenticationDto
                    {
                        MobileNumber = user.RegisteredMobileNo?.ToString() ?? string.Empty,
                        Name = user.Name,
                        AccessToken = "",
                        isOtpRequired = true,
                        ResponseMessage = responseMessage
                    };

                    await _activityLogger.LogAsync(responseMessage, userId: user.Id, email: user.Email, cnic: user.CNIC, description: responseMessage, appType: AppType.Mobile);
                    throw new UnAuthorizedException(responseMessage);

                    //return new SuccessResponse<MobileAuthenticationDto>(
                    //    newdto,
                    // "Authentication step completed.",
                    // "OTP Verification Required"
                    //);

                }
            }
        }

        SignInResult result = await _signInManager.PasswordSignInAsync(request.CNIC, request.Password, false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            await _activityLogger.LogAsync("InvalidPassword", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "Invalid password.", appType: AppType.Mobile);
            throw new UnAuthorizedException("Invalid password. Please try again.");

        }
        else if (user.IsOtpRequired)
        {
            string normalizedMobile = (user.RegisteredMobileNo ?? string.Empty)
                .Replace("-", "")              // remove dashes
                .Replace(" ", "")              // remove spaces
                .Trim();                       // remove leading/trailing whitespace

            if (normalizedMobile.StartsWith("0"))
            {
                normalizedMobile = "92" + normalizedMobile.Substring(1);
            }

            var random = new Random();
            var generateotp = random.Next(100000, 999999); // generates a 6-digit number

            string returnmsg = $"An OTP has been sent to your registered mobile number.{generateotp}";
            string sentmsg = $"{generateotp} is your OTP to sign in to the DHA Karachi Mobile App. Do not share it with anyone.";

            string smsresult = await _otpService.SendSmsAsync(normalizedMobile, sentmsg, cancellationToken);
            if (smsresult == "SUCCESSFUL")
            {
                var usernewOtp = new UserOtp
                {
                    UserId = Guid.Parse(user.Id),
                    CNIC = user.CNIC,
                    MobileNo = normalizedMobile,
                    OtpCode = generateotp.ToString(),
                    SentMessage = sentmsg,
                    IsVerified = false,
                    ExpiresAt = DateTime.Now.AddMinutes(5)
                };
                _context.UserOtps.Add(usernewOtp);
                await _context.SaveChangesAsync(cancellationToken);

                TimeSpan expiresIn = TimeSpan.FromMinutes(2); // 2-minute validity
                string verifyToken = await _authenticationService.GenerateTemporaryToken(user, "verify_otp", expiresIn);

                var finaldto= new MobileAuthenticationDto
                {
                    MobileNumber = MaskMobile(user.RegisteredMobileNo?.ToString()) ?? string.Empty,
                    Name = user.Name,
                    AccessToken = verifyToken,
                    isOtpRequired = true,
                    ResponseMessage = returnmsg
                };
                await _activityLogger.LogAsync("OTP Verification", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "OTP Verification Required", appType: AppType.Mobile);

                return new SuccessResponse<MobileAuthenticationDto>(
                        finaldto,
                     "Authentication step completed.",
                     "OTP Verification Required"
                    );
            }
            else
            {
                throw new UnAuthorizedException("Mobile Number not verified!");
            }
        }

        string accessToken = await _authenticationService.GenerateToken(user);
        IList<string> roles = await _userManager.GetRolesAsync(user);

        var dto = new MobileAuthenticationDto
        {
            MobileNumber = MaskMobile(user.RegisteredMobileNo) ?? string.Empty,
            Name = user.Name,
            AccessToken = accessToken,
            isOtpRequired = false,
            ResponseMessage = "Login successful. You are now signed in without OTP verification.",
        };

        if(!string.IsNullOrEmpty(request.DeviceId) && !string.IsNullOrEmpty(request.FCMToken))
        {
            await _activityLogger.DeviceAsync(UserId: Guid.Parse(user.Id), DeviceId: request.DeviceId, FCMToken: request.FCMToken, cancellationToken);
        }

        await _activityLogger.LogAsync("LoginSuccess", userId: user.Id, email: user.Email, cnic: user.CNIC, description: "User logged in successfully", appType: AppType.Mobile);

        return new SuccessResponse<MobileAuthenticationDto>(
            dto,
            "Authentication step completed.",
            "Login successful!"
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
