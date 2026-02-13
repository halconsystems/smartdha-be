using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.LiveMemberRegisteration;
public record LiveMemberRegisterationCommand : IRequest<SuccessResponse<RegisterationDto>>
{
    public string CNIC { get; set; } = default!;
}

public class LiveMemberRegisterationCommandHandler : IRequestHandler<LiveMemberRegisterationCommand, SuccessResponse<RegisterationDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISmsService _otpService;
    private readonly IProcedureService _sp;
    private readonly IApplicationDbContext _context;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMemberLookupService _memberLookupService;

    public LiveMemberRegisterationCommandHandler(UserManager<ApplicationUser> userManager, ISmsService otpService, IProcedureService sp, IApplicationDbContext context, IAuthenticationService authenticationService, IMemberLookupService memberLookupService)
    {
        _userManager = userManager;
        _otpService = otpService;
        _sp = sp;
        _context = context;
        _authenticationService = authenticationService;
        _memberLookupService = memberLookupService;
    }

    public async Task<SuccessResponse<RegisterationDto>> Handle(
    LiveMemberRegisterationCommand request,
    CancellationToken ct)
    {
        // 1) Normalize CNIC
        var cnic = request.CNIC?.Trim();
        if (string.IsNullOrWhiteSpace(cnic))
            throw new ConflictException("CNIC is required.");

        // 2) Check if user exists (idempotent)
        var user = await _userManager.Users
           .FirstOrDefaultAsync(u => u.CNIC == request.CNIC, ct);
        if (user != null)
        {
            if (user.PasswordHash == null || String.IsNullOrWhiteSpace(user.PasswordHash))
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

                string returnmsg = $"An OTP has been sent to your registered mobile number. Please use it to complete your sign-up for the DHA Karachi Mobile Application.{generateotp}";
                string sentmsg = $"{generateotp} is your One-Time Password (OTP) to verify your account on the DHA Karachi Mobile Application. Do not share this code with anyone.";

                string result = await _otpService.SendSmsAsync(normalizedMobile, sentmsg, ct);
                if (result == "SUCCESSFUL")
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
                    await _context.SaveChangesAsync(ct);

                    TimeSpan expiresIn = TimeSpan.FromMinutes(2); // 2-minute validity
                    string verifyuserToken = await _authenticationService.GenerateTemporaryToken(user, "verify_otp", expiresIn);

                    //return returnmsg;
                    RegisterationDto _registerationdto = new RegisterationDto
                    {
                        isOtpRequired = true,
                        AccessToken = verifyuserToken,
                        MobileNumber = MaskMobile(normalizedMobile),
                        ResponseMessage = returnmsg

                    };

                    return new SuccessResponse<RegisterationDto>(
                    _registerationdto,
                    returnmsg
                    );
                }
            }
            if (user.IsActive == false)
            {
                throw new ConflictException("Account already exists but is inactive. Please contact the administrator for assistance.");
            }
            if (user.IsDeleted == true)
            {
                throw new ConflictException("Account already exists but has been deleted. Please contact the administrator for assistance.");
            }
            if (user.IsVerified == false)
            {
                throw new ConflictException("Account already exists but is not verified. Please contact the administrator for assistance.");
            }
            throw new ConflictException("An account with this information already exists.");

        }

        // 4) Get member info (your existing call)
        var resultMember = await _memberLookupService.GetMemberByCnicAsync(cnic, ct);

        var memno = resultMember.Member.MemNo ?? "";
        var staffno = resultMember.Member.StaffNo ?? "";
        var name = resultMember.Member.Name ?? "";
        var cellno = NormalizePkMobile(resultMember.Member.CellNo ?? "");
        string message = resultMember.Member.Message ?? "No message";

        if (string.IsNullOrWhiteSpace(resultMember.Member.Name))
            throw new NotFoundException(message);

        if (string.IsNullOrWhiteSpace(cellno) || cellno.Length != 12 || !cellno.All(char.IsDigit))
            throw new ConflictException("Mobile number not found. Please visit DHA Head Office to update your registered mobile number.");

        // 5) Generate OTP (use crypto RNG; Random() is weak + repeats under load)
        var otp = GenerateSecureOtp();

        // 6) Create/Update DB state FAST (no SMS here)
        //    This part MUST be concurrency-safe -> relies on DB unique constraints
        try
        {
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    UserName = cnic,
                    NormalizedUserName = cnic,
                    Email = (resultMember.Member.Email ?? "").Trim(),
                    NormalizedEmail = (resultMember.Member.Email ?? "").Trim().ToUpper(),
                    MobileNo = cellno,
                    RegisteredMobileNo = cellno,
                    CNIC = cnic,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    AppType = AppType.Mobile,
                    UserType =
                        !string.IsNullOrEmpty(memno) && !string.IsNullOrEmpty(staffno)
                            ? UserType.StaffAndMember
                            : !string.IsNullOrEmpty(memno)
                                ? UserType.Member
                                : !string.IsNullOrEmpty(staffno)
                                    ? UserType.Employee
                                    : UserType.NonMember,

                    // OTP gate
                    RegistrationNo=cellno,
                    MEMPK= resultMember.Member.MemPk ?? "",
                    IsVerified = true,     // IMPORTANT: user is NOT verified until OTP
                    IsOtpRequired = true
                };

                var createRes = await _userManager.CreateAsync(user);
                if (!createRes.Succeeded)
                {
                    // If concurrent request created it first, re-fetch and continue
                    user = await _userManager.Users.FirstOrDefaultAsync(u => u.CNIC == cnic, ct);
                    if (user == null)
                        throw new ConflictException(string.Join(", ", createRes.Errors.Select(e => e.Description)));
                }
            }

            // Profile upsert
            var existingProfile = await _context.Set<UserMemberProfile>()
                .FirstOrDefaultAsync(x => x.UserId == user.Id, ct);

            if (existingProfile == null)
            {
                _context.Set<UserMemberProfile>().Add(new UserMemberProfile
                {
                    UserId = user.Id,
                    MemId = resultMember.Member.MemId ?? "0",
                    MemNo = resultMember.Member.MemNo ?? "",
                    StaffNo = resultMember.Member.StaffNo ?? "",
                    Category = resultMember.Member.Category ?? "",
                    Name = resultMember.Member.Name ?? "",
                    ApplicationDate = resultMember.Member.ApplicationDate ?? "",
                    NIC = resultMember.Member.NIC ?? "",
                    CellNo = cellno,
                    AllReplot = resultMember.Member.AllReplot ?? "",
                    MemPk = resultMember.Member.MemPk ?? "",
                    Email = resultMember.Member.Email ?? "",
                    DOB = resultMember.Member.DOB ?? "",
                    Message = resultMember.Member.Message ?? "No message"
                });
            }

            // Club memberships: ideally upsert or ignore duplicates
            if (resultMember.Clubs != null && resultMember.Clubs.Any())
            {
                var membershipNos = resultMember.Clubs.Select(x => x.MembershipNo).ToList();

                //var existing = await _context.ClubMemberships
                // .Where(x =>
                //     x.CNIC == cnic &&
                //     x.MembershipNo != null &&
                //     membershipNos.Contains(x.MembershipNo) &&
                //     x.IsDeleted != true)
                // .Select(x => x.MembershipNo!)
                // .ToListAsync(ct);

                //var toAdd = resultMember.Clubs
                //    .Where(c => !existing.Contains(c.MembershipNo))
                //    .Select(c => new ClubMembership
                //    {
                //        UserId = user.Id,
                //        MembershipNo = c.MembershipNo,
                //        Rank = c.Rank,
                //        Name = c.Name,
                //        CNIC = c.CNIC,
                //        MobileNumber = c.MobilNumber,
                //        OneInKid = c.OneInKid,
                //        BillStatus = c.BillStatus,
                //        Clube = c.Clube
                //    }).ToList();

                //if (toAdd.Any())
                //    _context.ClubMemberships.AddRange(toAdd);
            }

            // OTP upsert: ensure single active OTP per CNIC
            var activeOtp = await _context.UserOtps
                .Where(x => x.CNIC == cnic && x.IsVerified == false)
                .OrderByDescending(x => x.Created) // if you have Created
                .FirstOrDefaultAsync(ct);

            var smsText = $"{otp} is your One-Time Password (OTP) to verify your account on the DHA Karachi Mobile Application. Do not share this code with anyone.";

            if (activeOtp == null)
            {
                _context.UserOtps.Add(new UserOtp
                {
                    UserId = Guid.Parse(user.Id),
                    CNIC = cnic,
                    MobileNo = cellno,
                    OtpCode = otp,
                    SentMessage = smsText,
                    IsVerified = false,
                    ExpiresAt = DateTime.Now.AddMinutes(5)
                });
            }
            else
            {
                activeOtp.OtpCode = otp;
                activeOtp.SentMessage = smsText;
                activeOtp.MobileNo = cellno;
                activeOtp.ExpiresAt = DateTime.Now.AddMinutes(5);
                activeOtp.IsVerified = false;
            }

            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // concurrency: another thread inserted same CNIC/OTP etc.
            user = await _userManager.Users.FirstOrDefaultAsync(u => u.CNIC == cnic, ct);
            if (user == null)
                throw new ConflictException("Unable to process signup at the moment. Please try again.");
        }

        // 7) Send SMS AFTER DB saved (no locks held)
        var smsSent = await _otpService.SendSmsAsync(cellno,
            $"{otp} is your One-Time Password (OTP) to verify your account on the DHA Karachi Mobile Application. Do not share this code with anyone.",
            ct);

        if (smsSent != "SUCCESSFUL")
        {
            // IMPORTANT: we do NOT delete user; user is still NOT verified so cannot register/login.
            // user can retry via resend OTP endpoint.
            throw new ConflictException("Unable to send SMS at the moment. Please try again later.");
        }

        // 8) Issue temporary verify token (short)
        var verifyToken = await _authenticationService.GenerateTemporaryToken(user, "verify_otp", TimeSpan.FromMinutes(2));

        //return new SuccessResponse<RegisterationDto>(
        //    new RegisterationDto
        //    {
        //        isOtpRequired = true,
        //        AccessToken = verifyToken,
        //        MobileNumber = MaskMobile(cellno),
        //        ResponseMessage = "An OTP has been sent to your registered mobile number. Please use it to complete your sign-up for the DHA Karachi Mobile Application."
        //    },
        //    "OTP sent successfully"
        //);

        var _registerationfinaldto = new RegisterationDto
        {
            isOtpRequired = true,
            AccessToken = verifyToken,
            MobileNumber = MaskMobile(cellno),
            ResponseMessage = "An OTP has been sent to your registered mobile number. Please use it to complete your sign-up for the DHA Karachi Mobile Application."
        };

        return new SuccessResponse<RegisterationDto>(
            _registerationfinaldto,
            message
        );

    }

    // Helpers
    private static string NormalizePkMobile(string input)
    {
        var cell = (input ?? "")
            .Replace("-", "")
            .Replace(" ", "")
            .Trim();

        if (cell.StartsWith("0"))
            cell = "92" + cell[1..];

        return cell;
    }

    private static string GenerateSecureOtp()
    {
        // Crypto-secure 6-digit OTP
        // 100000 - 999999 inclusive
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var value = BitConverter.ToUInt32(bytes, 0);
        var otp = 100000 + (value % 900000);
        return otp.ToString();
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
