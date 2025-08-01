using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserById;
public class GetUserByIdQuery : IRequest<UserDto?>
{
    public string UserId { get; set; } = default!;
}

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .Include(u => u.ModuleAssignments)
                .ThenInclude(ma => ma.Module)
            .Where(u => u.Id == request.UserId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                CNIC = u.CNIC,
                MobileNo = u.MobileNo,
                MEMPK = u.MEMPK,
                AppType = u.AppType,
                UserType = u.UserType,
                RegisteredMobileNo = u.RegisteredMobileNo,
                RegisteredEmail = u.RegisteredEmail,
                RegistrationNo = u.RegistrationNo,
                RegistrationDate = u.RegistrationDate,
                IsActive = u.IsActive,
                IsDeleted = u.IsDeleted,
                IsVerified = u.IsVerified,
                IsOtpRequired = u.IsOtpRequired,
                ModuleAssignments = u.ModuleAssignments.Select(ma => new ModuleAssignmentDto
                {
                    ModuleId = ma.ModuleId,
                    ModuleName = ma.Module != null ? ma.Module.Name : string.Empty,
                  //  Description = ma.Module?.Description,
                  //  Title = ma.Module?.Title,
                  //  Remarks = ma.Module?.Remarks
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}
