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
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterNonMember;
public class RegisterGroundNonMemberCommand : IRequest<SuccessResponse<string>>
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
}

public class RegisterGroundNonMemberCommandHandler : IRequestHandler<RegisterGroundNonMemberCommand, SuccessResponse<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public RegisterGroundNonMemberCommandHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<SuccessResponse<string>> Handle(RegisterGroundNonMemberCommand request, CancellationToken cancellationToken)
    {
        // Start transaction
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Check if user already exists
            var existingUser = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.CNIC == request.CNIC, cancellationToken);

            if (existingUser != null)
                throw new RecordAlreadyExistException("A user with the same CNIC already exists.");

            // Create new Identity User
            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(), // Identity expects string PK
                Name = request.Name,
                UserName = request.CNIC,
                NormalizedUserName = request.CNIC,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                MobileNo = request.MobileNo,
                CNIC = request.CNIC,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                RegisteredMobileNo = request.MobileNo,
                IsOtpRequired = false,
                IsVerified = true,
                AppType = AppType.Web,
                UserType = UserType.NonMember,
                MEMPK = "-",
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
                throw new DBOperationException("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            var targetPurpose = await _context.MembershipPurposes
            .FirstOrDefaultAsync(p => p.Title == "Events / Club Access", cancellationToken);

            if (targetPurpose == null)
                throw new DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException("The specified membership purpose was not found.");

            // Store purposes
            var userPurpose = new UserMembershipPurpose
            {
                UserId = newUser.Id,
                PurposeId = targetPurpose.Id
            };
            await _context.UserMembershipPurposes.AddAsync(userPurpose, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            string finalmsg = "User successfully registered.";
            return SuccessResponse<string>.FromMessage(finalmsg);
        }
        catch (Exception ex)
        {
            // Rollback on error
            await transaction.RollbackAsync(cancellationToken);

            // Re-throw with proper context
            throw new ApplicationException($"Failed to register user: {ex.Message}", ex);
        }
    }
}


