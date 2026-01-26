using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Interface.Repository;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;
using ValidationException = DHAFacilitationAPIs.Application.Common.Exceptions.ValidationException;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<SuccessResponse<Guid>>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public List<ModuleSelectionDto> Modules { get; set; } = new();
}

public class ModuleSelectionDto
{
    public Guid ModuleId { get; set; }
    public List<SubModuleSelectionDto> SubModules { get; set; } = new();
    public List<Guid>? AssignedClubIds { get; set; }
    public List<Guid>? AssignedShopIds { get; set; }
}

public class SubModuleSelectionDto
{
    public Guid SubModuleId { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();  // e.g. Approve/Reject IDs
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<SuccessResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Start transaction
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1️⃣ Check if user already exists
            var existingUser = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Email == request.Email ||
                    u.UserName == request.UserName ||
                    u.CNIC == request.CNIC,
                    cancellationToken);

            if (existingUser != null)
                throw new RecordAlreadyExistException("A user with the same Email, Username, or CNIC already exists.");

            // 2️⃣ Create new Identity User
            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(), // Identity expects string PK
                Name = request.Name,
                UserName = request.UserName,
                NormalizedUserName = request.UserName.ToUpper(),
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                MobileNo = request.MobileNo,
                CNIC = request.CNIC,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                RegisteredMobileNo = request.MobileNo,
                IsOtpRequired=false,
                IsVerified=true,
                AppType = AppType.Web,
                UserType = UserType.Employee,
                MEMPK = "-",
            };

            var createResult = await _userManager.CreateAsync(newUser, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new DBOperationException($"User creation failed: {errors}");
            }

            // 3️⃣ Validate and assign Role
            if (!Guid.TryParse(request.RoleId, out var roleId))
                throw new DBOperationException("RoleId is not a valid GUID.");

            var role = await _context.AppRoles.FindAsync(new object[] { roleId }, cancellationToken);
            if (role == null)
                throw new NotFoundException($"Role with ID {roleId} was not found.");

            var userRole = new AppUserRole
            {
                UserId = newUser.Id,
                RoleId = role.Id
            };
            _context.AppUserRoles.Add(userRole);

            // 4️⃣ Assign Modules + SubModules + Permissions
            if (request.Modules != null)
            {
                foreach (var moduleSel in request.Modules)
                {
                    // ✅ Ensure module exists
                    var module = await _context.Modules
                        .FirstOrDefaultAsync(m => m.Id == moduleSel.ModuleId, cancellationToken);

                    if (module == null)
                        throw new NotFoundException($"Module with Id {moduleSel.ModuleId} does not exist.");

                    // User ↔ Module link
                    _context.UserModuleAssignments.Add(new UserModuleAssignment
                    {
                        UserId = newUser.Id,
                        ModuleId = moduleSel.ModuleId
                    });

                    // ✅ Special Case: ClubManagement requires club assignment
                    if (module.Value == "Club")
                    {
                        if (moduleSel.AssignedClubIds == null || !moduleSel.AssignedClubIds.Any())
                            throw new DBOperationException("At least one Club must be assigned for ClubManagement module.");

                        foreach (var clubId in moduleSel.AssignedClubIds)
                        {
                            _context.UserClubAssignments.Add(new UserClubAssignment
                            {
                                UserId = newUser.Id,
                                ClubId = clubId
                            });
                        }
                    }

                    if (module.Value == "Shop")
                    {
                        if (moduleSel.AssignedShopIds == null || !moduleSel.AssignedShopIds.Any())
                            throw new DBOperationException("At least one Shop must be assigned for ClubManagement module.");

                        foreach (var shopId in moduleSel.AssignedShopIds)
                        {
                            _context.UserShopAssignments.Add(new Domain.Entities.FMS.ShopAssignment
                            {
                                UserId = newUser.Id,
                                ShopId = shopId
                            });
                        }
                    }

                    // SubModules + Permissions
                    if (moduleSel.SubModules != null)
                    {
                        foreach (var subSel in moduleSel.SubModules)
                        {
                            _context.UserSubModuleAssignments.Add(new UserSubModuleAssignment
                            {
                                UserId = newUser.Id,
                                SubModuleId = subSel.SubModuleId
                            });

                            if (subSel.PermissionIds != null && subSel.PermissionIds.Any())
                            {
                                foreach (var permId in subSel.PermissionIds)
                                {
                                    _context.UserPermissionAssignments.Add(new UserPermissionAssignment
                                    {
                                        UserId = newUser.Id,
                                        PermissionId = permId
                                    });
                                }
                            }
                        }
                    }
                }
            }


            // 5️⃣ Save changes
            await _context.SaveChangesAsync(cancellationToken);

            // Commit the transaction
            await transaction.CommitAsync(cancellationToken);

            // Convert newUser.Id string → Guid safely
            var userGuid = Guid.TryParse(newUser.Id, out var parsedGuid)
                ? parsedGuid
                : Guid.Empty;

            return new SuccessResponse<Guid>(userGuid, "User created successfully.");
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

