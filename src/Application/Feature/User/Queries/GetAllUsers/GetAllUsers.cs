using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllUsers;
public record GetAllUsersQuery : IRequest<SuccessResponse<List<UserListDto>>>;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, SuccessResponse<List<UserListDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllUsersHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<SuccessResponse<List<UserListDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users.ToList();

        var result = new List<UserListDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserListDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                MobileNo = user.MobileNo,
                CNIC= user.CNIC,
                AppType = user.AppType.ToString(),
                UserType = user.UserType.ToString(),
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                Role = roles.FirstOrDefault() ?? "-"
            });
        }

        return new SuccessResponse<List<UserListDto>>(result);
    }
}

