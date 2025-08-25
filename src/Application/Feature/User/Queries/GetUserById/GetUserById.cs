using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserById;
public class GetUserByIdQuery : IRequest<SuccessResponse<UserDto>>
{
    public string UserId { get; set; } = default!;
}

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, SuccessResponse<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public GetUserByIdHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<SuccessResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId.ToString(), cancellationToken);

        if (user == null)
            throw new Exception("User not found");

        // 1️⃣ Get User Permissions
        var userPermissions = await _context.UserPermissions
            .Where(up => up.UserId == user.Id.ToString())
            .ToListAsync(cancellationToken);

        // 2️⃣ Get Module Assignments with SubModules
        var moduleAssignments = await _context.UserModuleAssignments
            .Include(ma => ma.Module)
                .ThenInclude(m => m.SubModules)
            .Where(ma => ma.UserId == user.Id)
            .ToListAsync(cancellationToken);

        // 3️⃣ Map to DTO
        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            CNIC = user.CNIC,
            MobileNo = user.MobileNo,
            MEMPK = user.MEMPK,
            AppType = user.AppType,
            UserType = user.UserType,
            RegisteredMobileNo = user.RegisteredMobileNo,
            RegisteredEmail = user.RegisteredEmail,
            RegistrationNo = user.RegistrationNo,
            RegistrationDate = user.RegistrationDate,
            IsActive = user.IsActive,
            IsDeleted = user.IsDeleted,
            IsVerified = user.IsVerified,
            IsOtpRequired = user.IsOtpRequired,

            ModuleAssignments = moduleAssignments.Select(ma => new ModuleAssignmentDto
            {
                ModuleId = ma.ModuleId,
                ModuleName = ma.Module?.Name ?? string.Empty,
                Description = ma.Module?.Description,
                Title = ma.Module?.Title,
                Remarks = ma.Module?.Remarks,

                SubModules = (ma.Module?.SubModules ?? new List<SubModule>())
                    .Select(sm => new SubModuleAssignmentDto
                    {
                        SubModuleId = sm.Id,
                        SubModuleName = sm.DisplayName,
                        RequiresPermission = sm.RequiresPermission,
                        Permissions = userPermissions
                            .Where(up => up.SubModuleId == sm.Id)
                            .Select(up => new UserPermissionDto
                            {
                                Id = up.Id,
                                AllowedActions = up.AllowedActions
                            }).ToList()
                    }).ToList()
            }).ToList()
        };

        return new SuccessResponse<UserDto>(userDto, "User details fetched successfully");
    }
}
