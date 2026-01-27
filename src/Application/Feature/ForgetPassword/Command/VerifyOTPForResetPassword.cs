using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DHAFacilitationAPIs.Application.Feature.ForgetPassword.Command;

public class PasswordResetTokenDto
{
    public string ResetToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}

//public record VerifyOtpForPasswordResetCommand(
//    string OtpCode
//) : IRequest<SuccessResponse<PasswordResetTokenDto>>;

public record VerifyOtpForPasswordResetCommand : IRequest<SuccessResponse<PasswordResetTokenDto>>
{
    // public string CNIC { get; init; } = default!;
    public string OtpCode { get; init; } = default!;
}

public class VerifyOtpForPasswordResetCommandHandler
    : IRequestHandler<VerifyOtpForPasswordResetCommand, SuccessResponse<PasswordResetTokenDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly ICurrentUserService _currentUserService;

    public VerifyOtpForPasswordResetCommandHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IAuthenticationService authenticationService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _userManager = userManager;
        _authenticationService = authenticationService;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<PasswordResetTokenDto>> Handle(
        VerifyOtpForPasswordResetCommand request,
        CancellationToken ct)
    {
        string UsedId = _currentUserService.UserId.ToString();

        var userDetails = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == UsedId, ct);
        if (userDetails == null)
            throw new KeyNotFoundException("User not found.");


        var otp = await _context.UserOtps
            .Where(x => x.CNIC == userDetails.CNIC && !x.IsVerified)
            .OrderByDescending(x => x.Created)
            .FirstOrDefaultAsync(ct);

        if (otp == null)
            throw new KeyNotFoundException("OTP not found.");

        if (otp.ExpiresAt < DateTime.Now)
            throw new ConflictException("OTP has expired.");

        if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(otp.OtpCode),
            Encoding.UTF8.GetBytes(request.OtpCode)))
            throw new ConflictException("Invalid OTP.");

        // Mark OTP as verified
        otp.IsVerified = true;

        // Generate TEMP password reset token (scoped)
        TimeSpan expiresIn = TimeSpan.FromMinutes(5);
        string resetToken = await _authenticationService.GenerateTemporaryToken(
            userDetails,
            purpose: "reset_password",
            expiresIn
        );

        await _context.SaveChangesAsync(ct);

        return new SuccessResponse<PasswordResetTokenDto>(
            new PasswordResetTokenDto
            {
                ResetToken = resetToken,
                ExpiresAt = DateTime.Now.Add(expiresIn)
            },
            "OTP verified successfully.",
            "Proceed to reset password."
        );
    }
}
