using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Interface.Repository;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; } = default!;
    public List<Guid>? ModuleIds { get; set; } = new();
    public List<SubModulePermissionInput>? SubModulePermissions { get; set; } = new(); // For "User"
}

public class SubModulePermissionInput
{
    public Guid SubModuleId { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
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

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.CNIC == request.CNIC, cancellationToken);

        if (existingUser != null)
        {
            if (existingUser.IsDeleted)
            {
                existingUser.IsDeleted = false;
                existingUser.Name = request.Name;
                existingUser.MobileNo = request.MobileNo;
                existingUser.CNIC = request.CNIC;
                existingUser.UserName = request.Email;
                existingUser.NormalizedUserName = request.Email.ToUpper();
                existingUser.NormalizedEmail = request.Email.ToUpper();

                var updateResult = await _userManager.UpdateAsync(existingUser);
                if (!updateResult.Succeeded)
                    throw new DBOperationException("Failed to restore user: " + string.Join(", ", updateResult.Errors.Select(e => e.Description)));

                await _userManager.RemovePasswordAsync(existingUser);
                var pwdResult = await _userManager.AddPasswordAsync(existingUser, request.Password);
                if (!pwdResult.Succeeded)
                    throw new DBOperationException("Failed to reset password: " + string.Join(", ", pwdResult.Errors.Select(e => e.Description)));

                var currentRoles = await _userManager.GetRolesAsync(existingUser);
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);

                if (!await _roleManager.RoleExistsAsync(request.Role))
                    throw new DBOperationException($"Role '{request.Role}' does not exist.");

                var roleAddResult = await _userManager.AddToRoleAsync(existingUser, request.Role);
                if (!roleAddResult.Succeeded)
                    throw new DBOperationException("Failed to assign role: " + string.Join(", ", roleAddResult.Errors.Select(e => e.Description)));

                await AssignModulesAndPermissions(existingUser.Id, request, cancellationToken);
                return Guid.Parse(existingUser.Id);
            }
            else
            {
                throw new RecordAlreadyExistException("User already exists.");
            }
        }

        // Create new user
        var newUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            UserName = request.Email,
            NormalizedUserName = request.Email.ToUpper(),
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            MobileNo = request.MobileNo,
            CNIC = request.CNIC,
            EmailConfirmed = true,
            AppType = AppType.Web,
            UserType = UserType.Employee
        };

        var createResult = await _userManager.CreateAsync(newUser, request.Password);
        if (!createResult.Succeeded)
            throw new DBOperationException("User creation failed: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));

        if (!await _roleManager.RoleExistsAsync(request.Role))
            throw new DBOperationException($"Role '{request.Role}' does not exist.");

        var roleResult = await _userManager.AddToRoleAsync(newUser, request.Role);
        if (!roleResult.Succeeded)
            throw new DBOperationException("Failed to assign role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));

        await AssignModulesAndPermissions(newUser.Id, request, cancellationToken);
        return Guid.Parse(newUser.Id);
    }

    private async Task AssignModulesAndPermissions(string userId, RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Assign modules
        if (request.ModuleIds != null)
        {
            foreach (var moduleId in request.ModuleIds)
            {
                _context.UserModuleAssignments.Add(new UserModuleAssignment
                {
                    UserId = userId,
                    ModuleId = moduleId
                });
            }
        }

        // Assign submodule permissions if role is "User"
        if (request.Role == "User" && request.SubModulePermissions != null)
        {
            foreach (var sub in request.SubModulePermissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleName = "User",
                    SubModuleId = sub.SubModuleId,
                    CanRead = sub.CanRead,
                    CanWrite = sub.CanWrite,
                    CanDelete = sub.CanDelete
                });
            }
        }
        else if(request.SubModulePermissions != null)
        {
            foreach (var sub in request.SubModulePermissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleName = request.Role,
                    SubModuleId = sub.SubModuleId,
                    CanRead = true,
                    CanWrite = true,
                    CanDelete = true
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

