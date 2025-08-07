using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterNonMember;
public class RegisterNonMemberCommand : IRequest<SuccessResponse<string>>
{
    [Required]
    public string CNIC { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public string Email { get; set; } = default!;

    [Required]
    public string MobileNo { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    [Required]
    public IFormFile CNICFrontImage { get; set; } = default!;

    [Required]
    public IFormFile CNICBackImage { get; set; } = default!;

    public IFormFile? SupportingDocument { get; set; }

    public List<Guid>? PurposeIds { get; set; } = new();  // Accept multiple

}

public class RegisterNonMemberCommandHandler : IRequestHandler<RegisterNonMemberCommand, SuccessResponse<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public RegisterNonMemberCommandHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _userManager = userManager;
        _context = context;
        _fileStorage = fileStorageService;
    }

    public async Task<SuccessResponse<string>> Handle(RegisterNonMemberCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if user already exists by CNIC
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.CNIC == request.CNIC, cancellationToken);

        if (existingUser != null)
        {
            if(existingUser.PhoneNumberConfirmed==false)
            {
                throw new UnAuthorizedException("Mobile Number not verified!");
            }

            // 2. Check existing verification request
            var existingRequest = await _context.NonMemberVerifications
                .Where(x => x.UserId == existingUser.Id)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingRequest != null)
            {
                var dto= existingRequest.Status switch
                {
                    VerificationStatus.Pending => "Your request is pending approval.",
                    VerificationStatus.Rejected => "Membership request was rejected. Please contact administrator.",
                    VerificationStatus.Approved => "User already registered.",
                    _ => "User already registered."
                };

                return SuccessResponse<string>.FromMessage(dto);
                

            }

            // No verification found but user exists — treat as registered
            string msg= "User already registered.";
            return SuccessResponse<string>.FromMessage(msg);
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            CNIC = request.CNIC,
            MobileNo = request.MobileNo,
            PhoneNumber = request.MobileNo,
            RegisteredMobileNo=request.MobileNo,
            PhoneNumberConfirmed = true,
            AppType = AppType.Mobile,
            UserType = UserType.NonMember,
            IsVerified = false,
            UserName = request.CNIC,
            Email = request.Email,
            IsOtpRequired = true,
            MEMPK = "-"
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new DBOperationException("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        // Save verification request
        var verification = new NonMemberVerification
        {
            UserId = user.Id,
            CNIC = request.CNIC,
            Status = VerificationStatus.Pending,
            Remarks = "Submitted with documents"
        };

        _context.NonMemberVerifications.Add(verification);

        // Store purposes
        if (request.PurposeIds != null && request.PurposeIds.Any())
        {
            var purposeLinks = request.PurposeIds.Select(pid => new UserMembershipPurpose
            {
                UserId = user.Id,
                PurposeId = pid
            }).ToList();

            await _context.UserMembershipPurposes.AddRangeAsync(purposeLinks, cancellationToken);
        }
       


        // Save documents
        var frontPath = await _fileStorage.SaveFileAsync(request.CNICFrontImage, "cnic", cancellationToken);
        var backPath = await _fileStorage.SaveFileAsync(request.CNICBackImage, "cnic", cancellationToken);
        string? supportPath = null;

        if (request.SupportingDocument != null)
        {
            supportPath = await _fileStorage.SaveFileAsync(request.SupportingDocument, "documents", cancellationToken);
        }

        var document = new NonMemberVerificationDocument
        {
            VerificationId = verification.Id,
            CNICFrontImagePath = frontPath,
            CNICBackImagePath = backPath,
            SupportingDocumentPath = supportPath
        };

        _context.NonMemberVerificationDocuments.Add(document);



        //Final call
        await _context.SaveChangesAsync(cancellationToken);




        string finalmsg= "Registration submitted successfully. Your request is pending review.";
        return SuccessResponse<string>.FromMessage(finalmsg);
    }
}

