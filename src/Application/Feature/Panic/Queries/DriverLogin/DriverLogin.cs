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

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.DriverLogin;
public record DriverLoginCommand(
    string CNIC,
    string Password,
    string DeviceId,
    string DeviceToken,
    string? DeviceName,
    string? DeviceOS,
    string? IPAddress
) : IRequest<SuccessResponse<MobileAuthenticationDto>>;
public class DriverLoginHandler
    : IRequestHandler<DriverLoginCommand, SuccessResponse<MobileAuthenticationDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly IApplicationDbContext _context;

    public DriverLoginHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationService authenticationService,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authenticationService = authenticationService;
        _context = context;
    }

    public async Task<SuccessResponse<MobileAuthenticationDto>> Handle(DriverLoginCommand request, CancellationToken ct)
    {
        // 1️⃣ Find user by CNIC
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.CNIC == request.CNIC, ct);

        if (user == null)
            throw new UnAuthorizedException("Invalid CNIC. Driver not found.");

        // 2️⃣ Must be Driver
        if (user.UserType != UserType.Driver)
            throw new UnAuthorizedException("Only registered drivers can use the Driver App.");

        // 3️⃣ Must be Mobile App user
        if (user.AppType != AppType.Mobile)
            throw new UnAuthorizedException("This user is not allowed to access the Driver App.");

        // 4️⃣ Check soft delete / active
        if (!user.IsActive)
            throw new UnAuthorizedException("Your account is inactive.");
        if (user.IsDeleted)
            throw new UnAuthorizedException("Your account has been deleted.");

        // 5️⃣ Login using Username (never CNIC!)
        var result = await _signInManager.PasswordSignInAsync(
            user.CNIC,
            request.Password,
            false,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
        {
            var audits = new UserLoginAudit
            {
                UserId = user?.Id ?? "",   // even if login failed
                LoginAt = DateTime.Now,
                IsSuccess = false,
                DeviceId = request.DeviceId,
                DeviceToken = request.DeviceToken,
                DeviceName = request.DeviceName,
                DeviceOS = request.DeviceOS,
                IPAddress = request.IPAddress
            };

            _context.UserLoginAudits.Add(audits);
            await _context.SaveChangesAsync(ct);
            throw new UnAuthorizedException("Invalid password. Please try again.");
        }
        // 6️⃣ Generate driver-specific token
        string token = await _authenticationService.GenerateDriverToken(user);

        // 7️⃣ Response DTO
        var dto = new MobileAuthenticationDto
        {
            MobileNumber = MaskMobile(user.RegisteredMobileNo ?? ""),
            Name = user.Name,
            AccessToken = token,
            isOtpRequired = false,
            ResponseMessage = "Login successful."
        };

        var audit = new UserLoginAudit
        {
            UserId = user?.Id ?? "",   // even if login failed
            LoginAt = DateTime.Now,
            IsSuccess = true,
            DeviceId = request.DeviceId,
            DeviceName = request.DeviceName,
            DeviceOS = request.DeviceOS,
            DeviceToken = request.DeviceToken,
            IPAddress = request.IPAddress
        };

        _context.UserLoginAudits.Add(audit);
        await _context.SaveChangesAsync(ct);

        return new SuccessResponse<MobileAuthenticationDto>(
            dto,
            "Driver login successful."
        );
    }

    private static string MaskMobile(string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            return "";

        var last4 = mobile.Length > 4 ? mobile[^4..] : mobile;
        return new string('*', mobile.Length - last4.Length) + last4;
    }
}

