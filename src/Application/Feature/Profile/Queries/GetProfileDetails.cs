using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserDashboard;
using DHAFacilitationAPIs.Application.Interface.Service;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Profile.Queries;

public record GetProfileDetailsQuery : IRequest<ProfileDTO>;
public class GetProfileDetailsHandler : IRequestHandler<GetProfileDetailsQuery, ProfileDTO>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly ISmsService _otpService;
    private readonly IApplicationDbContext _context;
    private readonly IActivityLogger _activityLogger;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorageService;

    public GetProfileDetailsHandler(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationService authenticationService,
        ISmsService otpService,
        IApplicationDbContext context,
        IActivityLogger activityLogger,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage
        )

    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authenticationService = authenticationService;
        _otpService = otpService;
        _context = context;
        _activityLogger = activityLogger;
        _currentUser = currentUser;
        _fileStorageService = fileStorage;
    }
    public async Task<ProfileDTO> Handle(GetProfileDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.UserId.ToString();
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);

        if (user == null)
        {
            throw new UnAuthorizedException("Profile Daetails Not Fount. Please verify and try again.");

        }

        var userimage = _context.UserImages.FirstOrDefault(x => x.UserId == Guid.Parse(user.Id));

        var publicUrl = string.IsNullOrWhiteSpace(userimage?.ImageURL)
                ? string.Empty
                : _fileStorageService.GetPublicUrl(userimage.ImageURL) ?? string.Empty;
        var result = new ProfileDTO
        {

            Name = user.Name,
            Email = user.Email,
            Cnic = user.CNIC,
            MemPk = user.MEMPK,
            PhoneNumber = user.PhoneNumber,
            RegistteredMobileNumber = user.RegisteredMobileNo,
            RegistteredEmail = user.RegisteredEmail,
            IsMember =
                string.IsNullOrWhiteSpace(user.MEMPK) || user.MEMPK == "-"
                    ? false
                    : true,
            //StaffNo = user.StaffNo
            ProfileImage = publicUrl

        };
        return result;
    }


}
