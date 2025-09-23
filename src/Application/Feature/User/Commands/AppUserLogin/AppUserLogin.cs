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
public record AppUserLoginCommand : IRequest<SuccessResponse<MobileAuthenticationDto>>
{
    public string CNIC { get; set; } = default!;
    public string Password { get; set; } = default!;
}
public class AppUserLoginHandler : IRequestHandler<AppUserLoginCommand, SuccessResponse<MobileAuthenticationDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly ISmsService _otpService;
    private readonly IApplicationDbContext _context;

    public AppUserLoginHandler(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationService authenticationService,
        ISmsService otpService,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authenticationService = authenticationService;
        _otpService = otpService;
        _context = context;
    }
    public async Task<SuccessResponse<MobileAuthenticationDto>> Handle(AppUserLoginCommand request, CancellationToken cancellationToken)
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
        else if (user.PhoneNumberConfirmed == false)
        {
            throw new UnAuthorizedException("Mobile Number not verified!");
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
                        VerificationStatus.Pending => "Your request is pending approval.",
                        VerificationStatus.Rejected => "Membership request was rejected. Please contact administrator.",
                        VerificationStatus.Approved => "User already registered.",
                        _ => "User already registered."
                    };

                    var newdto= new MobileAuthenticationDto
                    {
                        MobileNumber = user.RegisteredMobileNo?.ToString() ?? string.Empty,
                        Name = user.Name,
                        AccessToken = "",
                        isOtpRequired = true,
                        ResponseMessage = responseMessage
                    };

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
            throw new UnAuthorizedException("Invalid Password");
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

            string returnmsg = $"An OTP has been sent to the mobile number registered with your membership {generateotp}";
            string sentmsg =  generateotp + " is your OTP to sign-in DHA Karachi mobile application.";
            string smsresult = await _otpService.SendSmsAsync(normalizedMobile, sentmsg, cancellationToken);
            //if (smsresult == "SUCCESSFUL")
            //{
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

                var finaldto= new MobileAuthenticationDto
                {
                    MobileNumber = user.RegisteredMobileNo?.ToString() ?? string.Empty,
                    Name = user.Name,
                    AccessToken = "",
                    isOtpRequired = true,
                    ResponseMessage = returnmsg
                };

                return new SuccessResponse<MobileAuthenticationDto>(
                        finaldto,
                     "Authentication step completed.",
                     "OTP Verification Required"
                    );
            //}
            //else
            //{
            //    throw new UnAuthorizedException("Mobile Number not verified!");
            //}
        }

        string accessToken = await _authenticationService.GenerateToken(user);
        IList<string> roles = await _userManager.GetRolesAsync(user);

        var dto = new MobileAuthenticationDto
        {
            MobileNumber = user.RegisteredMobileNo ?? string.Empty,
            Name = user.Name,
            AccessToken = accessToken,
            isOtpRequired = false,
            ResponseMessage = "Login successful! You are now authenticated without OTP."
        };

        return new SuccessResponse<MobileAuthenticationDto>(
            dto,
            "Authentication step completed.",
            "Login successful!"
        );

        //var dto = new MobileAuthenticationDto
        //{
        //    MobileNumber = user.RegisteredMobileNo?.ToString() ?? string.Empty,
        //    Name = user.Name,
        //    AccessToken = token,
        //    isOtpRequired = false,
        //    ResponseMessage = "Login successful! You are now authenticated without OTP."
        //};

        //return new SuccessResponse<MobileAuthenticationDto>(
        //                dto,
        //             "Authentication step completed.",
        //             "Login successful! You are now authenticated without OTP."
        //            );
    }
}
