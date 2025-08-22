using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllUsers;
public record GetAllUsersQuery : IRequest<SuccessResponse<List<UserListDto>>>;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, SuccessResponse<List<UserListDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public GetAllUsersHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<SuccessResponse<List<UserListDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
     .AsNoTracking()
     .ToListAsync(cancellationToken);

        var result = new List<UserListDto>();

        foreach (var user in users)
        {
            // Fetch role(s) from AppRoles via AppUserRoles
            var roleNames = await _context.AppUserRoles
                .Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.Name)
                .ToListAsync(cancellationToken);

            result.Add(new UserListDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                MobileNo = user.MobileNo,
                CNIC = user.CNIC,
                AppType = user.AppType.ToString(),
                UserType = user.UserType.ToString(),
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                Role = roleNames.Any() ? string.Join(", ", roleNames) : "-" // handles multiple roles
            });
        }

        return new SuccessResponse<List<UserListDto>>(result);

    }
}

