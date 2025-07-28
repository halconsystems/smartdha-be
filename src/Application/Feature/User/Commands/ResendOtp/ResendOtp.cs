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
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.ResendOtp;
public record ResendOtpCommand(string CNIC) : IRequest<SuccessResponse<string>>;

public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, SuccessResponse<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;
    private readonly ISmsService _otpService;

    public ResendOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context,
        ISmsService otpService)
    {
        _userManager = userManager;
        _context = context;
        _otpService = otpService;
    }

    public async Task<SuccessResponse<string>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.CNIC == request.CNIC, cancellationToken);

        if (existingUser == null)
            throw new NotFoundException("User with the provided CNIC does not exist.");

        //if (existingUser.PhoneNumberConfirmed)
        //    throw new ConflictException("User already verified.");

        // Normalize mobile number
        string normalizedMobile = existingUser.MobileNo
            .Replace("-", "")
            .Replace(" ", "")
            .Trim();

        if (normalizedMobile.StartsWith("0"))
            normalizedMobile = "92" + normalizedMobile.Substring(1);

        // Generate OTP
        var otp = new Random().Next(100000, 999999);
        string sentMessage = $"{otp} is your OTP to sign-up DHA Karachi mobile application.";
        string returnMessage = $"An OTP has been sent to the mobile number registered with your membership {otp}";

        // Send SMS
        string result = await _otpService.SendSmsAsync(normalizedMobile, sentMessage, cancellationToken);
        if (!result.Equals("SUCCESSFUL", StringComparison.OrdinalIgnoreCase))
            throw new Exception("Failed to send OTP. Please try again later.");

        // Save OTP
        var userOtp = new UserOtp
        {
            UserId = Guid.Parse(existingUser.Id),
            CNIC = existingUser.CNIC,
            MobileNo = normalizedMobile,
            OtpCode = otp.ToString(),
            SentMessage = sentMessage,
            IsVerified = false,
            ExpiresAt = DateTime.Now.AddMinutes(5)
        };

        _context.UserOtps.Add(userOtp);
        await _context.SaveChangesAsync(cancellationToken);

        return SuccessResponse<string>.FromMessage(returnMessage);
    }
}

