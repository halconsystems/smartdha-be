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
    public string PhoneNumber { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; } = default!;
}
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser != null)
        {
            if (existingUser.IsDeleted)
            {
                // Restore soft-deleted user
                existingUser.IsDeleted = false;
                existingUser.Name = request.Name;
                existingUser.PhoneNumber = request.PhoneNumber;
                existingUser.CNIC = request.CNIC;
                existingUser.UserName = request.Email;
                existingUser.NormalizedUserName = request.Email.ToUpper();
                existingUser.NormalizedEmail = request.Email.ToUpper();

                var updateResult = await _userManager.UpdateAsync(existingUser);
                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.Select(e => e.Description);
                    throw new DBOperationException("Failed to restore user. Errors: " + string.Join(", ", errors));
                }

                await _userManager.RemovePasswordAsync(existingUser);
                var passwordResult = await _userManager.AddPasswordAsync(existingUser, request.Password);
                if (!passwordResult.Succeeded)
                {
                    var errors = passwordResult.Errors.Select(e => e.Description);
                    throw new DBOperationException("Failed to set password for restored user. Errors: " + string.Join(", ", errors));
                }

                if (!string.IsNullOrWhiteSpace(request.Role))
                {
                    var currentRoles = await _userManager.GetRolesAsync(existingUser);
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                    }

                    if (!await _roleManager.RoleExistsAsync(request.Role))
                        throw new DBOperationException($"Role '{request.Role}' does not exist.");

                    var roleResult = await _userManager.AddToRoleAsync(existingUser, request.Role);
                    if (!roleResult.Succeeded)
                    {
                        var errors = roleResult.Errors.Select(e => e.Description);
                        throw new DBOperationException("Failed to assign role to restored user. Errors: " + string.Join(", ", errors));
                    }
                }

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
            PhoneNumber = request.PhoneNumber,
            CNIC = request.CNIC,
            EmailConfirmed = true,
            UserType = UserType.Web
        };

        var createResult = await _userManager.CreateAsync(newUser, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description);
            throw new DBOperationException("Failed to create user. Errors: " + string.Join(", ", errors));
        }

        if (!await _roleManager.RoleExistsAsync(request.Role))
            throw new DBOperationException($"Role '{request.Role}' does not exist.");

        var roleResultAdd = await _userManager.AddToRoleAsync(newUser, request.Role);
        if (!roleResultAdd.Succeeded)
        {
            var errors = roleResultAdd.Errors.Select(e => e.Description);
            throw new DBOperationException("Failed to assign role to new user. Errors: " + string.Join(", ", errors));
        }



        return Guid.Parse(newUser.Id);
    }
}
