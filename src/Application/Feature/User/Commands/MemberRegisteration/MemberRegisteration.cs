using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Interface.Repository;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.MemberRegisteration;
public record MemberRegisterationCommand : IRequest<SuccessResponse<RegisterationDto>>
{
    public string CNIC { get; set; } = default!;
}

public class MemberRegisterationCommandHandler : IRequestHandler<MemberRegisterationCommand, SuccessResponse<RegisterationDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISmsService _otpService;
    private readonly IProcedureService _sp;
    private readonly IApplicationDbContext _context;
    private readonly IAuthenticationService _authenticationService;

    public MemberRegisterationCommandHandler(UserManager<ApplicationUser> userManager, ISmsService otpService, IProcedureService sp, IApplicationDbContext context, IAuthenticationService authenticationService)
    {
        _userManager = userManager;
        _otpService = otpService;
        _sp = sp;
        _context = context;
        _authenticationService = authenticationService;
    }

    public async Task<SuccessResponse<RegisterationDto>> Handle(MemberRegisterationCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.CNIC == request.CNIC, cancellationToken);
        if (existingUser != null)
        {
            if (existingUser.PasswordHash == null || String.IsNullOrWhiteSpace(existingUser.PasswordHash))
            {
                string normalizedMobile = (existingUser.RegisteredMobileNo ?? string.Empty)
                .Replace("-", "")              // remove dashes
                .Replace(" ", "")              // remove spaces
                .Trim();                       // remove leading/trailing whitespace

                if (normalizedMobile.StartsWith("0"))
                {
                    normalizedMobile = "92" + normalizedMobile.Substring(1);
                }

                var random = new Random();
                var generateotp = random.Next(100000, 999999); // generates a 6-digit number

                string returnmsg = $"An OTP has been sent to your registered mobile number. Please use it to complete your sign-up for the DHA Karachi Mobile Application.{generateotp}";
                string sentmsg = $"{generateotp} is your One-Time Password (OTP) to verify your account on the DHA Karachi Mobile Application. Do not share this code with anyone.";

                string result = await _otpService.SendSmsAsync(normalizedMobile, sentmsg, cancellationToken);
                if (result == "SUCCESSFUL")
                {
                    var usernewOtp = new UserOtp
                    {
                        UserId = Guid.Parse(existingUser.Id),
                        CNIC = existingUser.CNIC,
                        MobileNo = normalizedMobile,
                        OtpCode = generateotp.ToString(),
                        SentMessage = sentmsg,
                        IsVerified = false,
                        ExpiresAt = DateTime.Now.AddMinutes(5)
                    };
                    _context.UserOtps.Add(usernewOtp);
                    await _context.SaveChangesAsync(cancellationToken);

                    TimeSpan expiresIn = TimeSpan.FromMinutes(2); // 2-minute validity
                    string verifyToken = await _authenticationService.GenerateTemporaryToken(existingUser, "verify_otp", expiresIn);

                    //return returnmsg;
                    RegisterationDto _registerationdto = new RegisterationDto
                    {
                        isOtpRequired = true,
                        AccessToken = verifyToken,
                        MobileNumber = MaskMobile(normalizedMobile),
                        ResponseMessage = returnmsg

                    };

                    return new SuccessResponse<RegisterationDto>(
                    _registerationdto,
                    returnmsg
                    );
                }
            }
            if (existingUser.IsActive == false)
            {
                throw new ConflictException("Account already exists but is inactive. Please contact the administrator for assistance.");
            }
            if (existingUser.IsDeleted == true)
            {
                throw new ConflictException("Account already exists but has been deleted. Please contact the administrator for assistance.");
            }
            if (existingUser.IsVerified == false)
            {
                throw new ConflictException("Account already exists but is not verified. Please contact the administrator for assistance.");
            }
            throw new ConflictException("An account with this information already exists.");

        }

        var p = new DynamicParameters();
        p.Add("@CNICNO", request.CNIC, DbType.String, size: 150);

        // Output parameters
        //p.Add("@MEMPK", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        //p.Add("@Name", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        p.Add("@OTP", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@OutCellNo", dbType: DbType.String, size: 15, direction: ParameterDirection.Output);
        p.Add("@msg", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);
        p.Add("@Name", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);
        p.Add("@Email", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);

        // Execute stored procedure
        await _sp.ExecuteAsync(
            "USP_ApplyForRegistration_CNIC",
            p,
            cancellationToken: cancellationToken,
            "DefaultConnection"
        );

        // Read output parameters
        //string memPk = p.Get<string>("@MEMPK") ?? "0";
        //string name = p.Get<string>("@Name") ?? "";
        string userOtp = p.Get<int>("@OTP").ToString();
        string outCellNo = p.Get<string>("@OutCellNo") ?? "No Number Found";
        string message = p.Get<string>("@msg") ?? "No message";
        string FullName = p.Get<string>("@Name") ?? "No message";
        string requestEmail = p.Get<string>("@Email") ?? "No message";

        outCellNo = (outCellNo ?? string.Empty)
                .Replace("-", "")              // remove dashes
                .Replace(" ", "")              // remove spaces
                .Trim();                       // remove leading/trailing whitespace

        if (outCellNo.StartsWith("0"))
        {
            outCellNo = "92" + outCellNo.Substring(1);
        }
        if (int.TryParse(userOtp, out var otp) && otp > 0)
        {
            var newUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            UserName = request.CNIC,
            NormalizedUserName = request.CNIC,//email.ToUpper(),
            Email = email,
            NormalizedEmail = email.ToUpper(),
            MobileNo = cellno,
            CNIC = request.CNIC,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            AppType = AppType.Mobile,
            UserType = !string.IsNullOrEmpty(memno) ? UserType.Member : !string.IsNullOrEmpty(staffno) ? UserType.Employee : UserType.NonMember,
            RegisteredMobileNo = cellno,
            IsVerified = true,
            IsOtpRequired = true,
            MEMPK = mempk//"DHAM-97563"
        };
        await _userManager.CreateAsync(newUser);
        // Send OTP to MobileNo
        string sentmsg = $"{userOtp} is your OTP for DHA Karachi Mobile App. Do not share it.";

            var result = await _otpService.SendSmsAsync(outCellNo, sentmsg, cancellationToken);
            if (result == "SUCCESSFUL")
            {
                var usernewOtp = new UserOtp
                {
                    UserId = Guid.Parse(newUser.Id),
                    CNIC = newUser.CNIC,
                    MobileNo = outCellNo,
                    OtpCode = userOtp.ToString(),
                    SentMessage = sentmsg,
                    IsVerified = false,
                    ExpiresAt = DateTime.Now.AddMinutes(5)
                };
                _context.UserOtps.Add(usernewOtp);
                await _context.SaveChangesAsync(cancellationToken);

                TimeSpan expiresIn = TimeSpan.FromMinutes(2); // 2-minute validity
                string verifyToken = await _authenticationService.GenerateTemporaryToken(newUser, "verify_otp", expiresIn);

                //return returnmsg;
                RegisterationDto _registerationdto = new RegisterationDto
                {
                    isOtpRequired = true,
                    AccessToken = verifyToken,
                    MobileNumber = MaskMobile(outCellNo),
                    ResponseMessage = message

                };

                return new SuccessResponse<RegisterationDto>(
                _registerationdto,
                message
                );

                //return SuccessResponse<string>.FromMessage(message);
            }
            else
            {
                throw new BadRequestException(message);
            }

        }
        else
        {
            throw new NotFoundException(message);
        }
    }

    public static string MaskMobile(string normalizedMobile)
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


