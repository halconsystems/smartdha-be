using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetCurrentUserProfile;
public class GetCurrentUserProfileQuery : IRequest<SuccessResponse<UserProfileDto>>
{
}

public class GetCurrentUserProfileQueryHandler : IRequestHandler<GetCurrentUserProfileQuery, SuccessResponse<UserProfileDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetCurrentUserProfileQueryHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SuccessResponse<UserProfileDto>> Handle(GetCurrentUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new Exception("User not found.");

        var userProfile = new UserProfileDto
        {
            FullName = user.Name,
            Email = user.Email ?? string.Empty,
            UserType = user.UserType.ToString(),
            CNIC = user.CNIC,
            MobileNo= user.MobileNo
        };

        return new SuccessResponse<UserProfileDto>(
            data: userProfile,
            detail: "User profile retrieved successfully.",
            title: "Success"
        );
    }
}
