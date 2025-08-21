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
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.MemberRegisteration;
public record MemberRegisterationCommand : IRequest<SuccessResponse<string>>
{
    public string CNIC { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
}

public class MemberRegisterationCommandHandler : IRequestHandler<MemberRegisterationCommand, SuccessResponse<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISmsService _otpService;
    private readonly IProcedureService _sp;
    private readonly IApplicationDbContext _context;

    public MemberRegisterationCommandHandler(UserManager<ApplicationUser> userManager, ISmsService otpService, IProcedureService sp, IApplicationDbContext context)
    {
        _userManager = userManager;
        _otpService = otpService;
        _sp = sp;
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(MemberRegisterationCommand request, CancellationToken cancellationToken)
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

                string returnmsg = $"An OTP has been sent to the mobile number registered with your membership {generateotp}";
                string sentmsg = generateotp + " is your OTP to sign-up DHA Karachi mobile application.";
                string result=await _otpService.SendSmsAsync(normalizedMobile, sentmsg, cancellationToken);
                if(result == "SUCCESSFUL")
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

                    //return returnmsg;
                    return SuccessResponse<string>.FromMessage(returnmsg);
                }
            }
            if (existingUser.IsActive == false)
            {
                throw new ConflictException("User already exists but InActive contact with administrator!");
            }
            if (existingUser.IsDeleted == true)
            {
                throw new ConflictException("User already exists but deleted contact with administrator!");
            }
            if (existingUser.IsVerified == false)
            {
                throw new ConflictException("User already exists but not verified contact with administrator!");
            }
            throw new ConflictException("User already exists");
        }
       
        var p = new DynamicParameters();
        p.Add("@CNICNO", request.CNIC, DbType.String, size: 150);
        p.Add("@CellNo", request.MobileNo, DbType.String, size: 15);

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
            "USP_ApplyForRegistration_Temp",
            p,
            cancellationToken: cancellationToken,
            "DefaultConnection"
        );

        // Read output parameters
        //string memPk = p.Get<string>("@MEMPK") ?? "0";
        //string name = p.Get<string>("@Name") ?? "";
        string userOtp = p.Get<int>("@OTP").ToString();
        string outCellNo = p.Get<string>("@OutCellNo") ?? request.MobileNo;
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
                Name = FullName,
                UserName = request.CNIC,
                NormalizedUserName = requestEmail.ToUpper(),
                Email = requestEmail,
                NormalizedEmail = requestEmail.ToUpper(),
                MobileNo = request.MobileNo,
                CNIC = request.CNIC,
                EmailConfirmed = true,
                PhoneNumberConfirmed=true,
                TwoFactorEnabled = false,
                AppType = AppType.Mobile,
                UserType = UserType.Member,
                RegisteredMobileNo= outCellNo,
                IsVerified=true,
                IsOtpRequired=true,
                MEMPK= "DHAM-97563"
            };
            await _userManager.CreateAsync(newUser);
            // Send OTP to MobileNo
            string sentmsg = userOtp + " is your OTP to sign-up DHA Karachi mobile application.";

            var result =await _otpService.SendSmsAsync(outCellNo, sentmsg, cancellationToken);
            //if (result == "SUCCESSFUL" )
            //{
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

                return SuccessResponse<string>.FromMessage(message);
            //}
            //else
            //{
            //    throw new BadRequestException(message);
            //}
            
        }
        else
        {
            throw new NotFoundException(message);
        }
    }
}

