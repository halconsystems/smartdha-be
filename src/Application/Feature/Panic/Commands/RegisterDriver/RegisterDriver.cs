using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.RegisterDriver;
public record RegisterDriverCommand(
    string Email,
    string Name,
    string MobileNo,
    string Password,
    string CNIC
) : IRequest<SuccessResponse<Guid>>;
public class RegisterDriverCommandHandler
    : IRequestHandler<RegisterDriverCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegisterDriverCommandHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<SuccessResponse<Guid>> Handle(RegisterDriverCommand request, CancellationToken ct)
    {

        var existingDriver = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Email == request.Email ||
                    u.CNIC == request.CNIC,
                    ct);

        if (existingDriver != null)
        {
            if (existingDriver.Email == request.Email)
                throw new RecordAlreadyExistException($"Email '{request.Email}' is already registered.");

            if (existingDriver.CNIC == request.CNIC)
                throw new RecordAlreadyExistException($"CNIC '{request.CNIC}' is already registered.");
        }

        await using var trx = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            // 0️⃣ Unique checks for Email + CNIC
            

            // 1️⃣ Auto-create unique Login + Email
            string username = request.CNIC;

            // 2️⃣ Ensure “Driver” role exists
            const string driverRoleName = "Driver";

            if (!await _roleManager.RoleExistsAsync(driverRoleName))
            {
                var newRole = new IdentityRole(driverRoleName);
                var roleResult = await _roleManager.CreateAsync(newRole);

                if (!roleResult.Succeeded)
                    throw new Exception("Failed to create Driver role.");
            }

            // 3️⃣ Create Identity User
            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                Email = request.Email,
                NormalizedEmail = request.Email,
                MobileNo = request.MobileNo,
                RegisteredMobileNo = request.MobileNo,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CNIC = request.CNIC,          // static

                // IMPORTANT
                UserType = UserType.Driver,
                AppType = AppType.Mobile,
                MEMPK = "-",         // static
                IsOtpRequired = false,
                IsVerified = true
            };

            var createResult = await _userManager.CreateAsync(newUser, request.Password);
            if (!createResult.Succeeded)
            {
                string errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new Exception("Driver creation failed: " + errors);
            }

            // 4️⃣ Assign “Driver” role
            var roleAssign = await _userManager.AddToRoleAsync(newUser, driverRoleName);
            if (!roleAssign.Succeeded)
                throw new Exception("Failed to assign Driver role.");

            // NO MODULES, SUBMODULES, PERMISSIONS — Driver doesn't need those.

            await _context.SaveChangesAsync(ct);
            await trx.CommitAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                Guid.Parse(newUser.Id),
                "Driver registered successfully."
            );
        }
        catch (Exception ex)
        {
            await trx.RollbackAsync(ct);
            throw new ApplicationException($"RegisterDriver failed: {ex.Message}", ex);
        }
    }
}

