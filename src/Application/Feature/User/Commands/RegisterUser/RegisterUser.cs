using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Interface.Repository;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

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
    public string MEMPK { get; set; } = default!;
    public List<ModuleSelectionDto> Modules { get; set; } = new();
}

public class ModuleSelectionDto
{
    public Guid ModuleId { get; set; }
    public List<SubModuleSelectionDto> SubModules { get; set; } = new();
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
        var existingUser = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u =>
                u.Email == request.Email ||
                u.UserName == request.UserName ||
                u.CNIC == request.CNIC,
                cancellationToken);

        if (existingUser != null)
        {
            throw new RecordAlreadyExistException("User already exists.");
        }

        // Create new user
        var newUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(), // Identity user uses string Id
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
            AppType = AppType.Web,
            UserType = UserType.Employee,
            MEMPK = request.MEMPK
        };

        var createResult = await _userManager.CreateAsync(newUser, request.Password);
        if (!createResult.Succeeded)
            throw new DBOperationException("User creation failed: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));

        // Assign Role
        var role = await _context.AppRoles.FindAsync(new object[] { request.RoleId }, cancellationToken);
        if (role == null) throw new Exception("Role not found.");

        var userRole = new AppUserRole
        {
            UserId = newUser.Id,
            RoleId = role.Id
        };
        _context.AppUserRoles.Add(userRole);

        // Assign Modules + SubModules + Permissions
        if (request.Modules != null)
        {
            foreach (var moduleSel in request.Modules)
            {
                // User ↔ Module link
                var assignment = new UserModuleAssignment
                {
                    UserId = newUser.Id,
                    ModuleId = moduleSel.ModuleId
                };
                _context.UserModuleAssignments.Add(assignment);

                // SubModules + Permissions
                if (moduleSel.SubModules != null)
                {
                    foreach (var subSel in moduleSel.SubModules)
                    {
                        //if (subSel.PermissionIds == null || !subSel.PermissionIds.Any())
                        //    continue;
                        if (subSel.PermissionIds != null && subSel.PermissionIds.Any())
                        {
                            var userPermission = new UserPermission
                            {
                                UserId = newUser.Id,
                                SubModuleId = subSel.SubModuleId,
                                AllowedActions = string.Join(",", subSel.PermissionIds) // CSV
                            };

                            _context.UserPermissions.Add(userPermission);
                        }
                        else
                        {
                            // Case 2️⃣: SubModule has NO permissions (e.g. Dashboard)
                            // Create an entry so we know the user has access to this submodule
                            var userPermission = new UserPermission
                            {
                                UserId = newUser.Id,
                                SubModuleId = subSel.SubModuleId,
                                AllowedActions = string.Empty // means "has access, but no fine-grained actions"
                            };

                            _context.UserPermissions.Add(userPermission);
                        }
                    }
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Convert newUser.Id string → Guid safely
        var userGuid = Guid.TryParse(newUser.Id, out var parsedGuid)
            ? parsedGuid
            : Guid.Empty;

        return new SuccessResponse<Guid>(userGuid, "User created successfully.");
    }

}

