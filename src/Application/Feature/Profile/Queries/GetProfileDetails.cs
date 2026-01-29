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
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorageService;

    public GetProfileDetailsHandler(UserManager<ApplicationUser> userManager,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage
        )

    {
        _userManager = userManager;
        _context = context;
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
        var getMemberProfiles = await _context.UserMemberProfiles.Where(x=>x.UserId == currentUserId).FirstOrDefaultAsync(cancellationToken);
      
        var result = new ProfileDTO
        {
            Name = user.Name,
            Email = user.Email,
            Cnic = user.CNIC,
            MemPk = getMemberProfiles?.MemPk,
            PhoneNumber = user.PhoneNumber,
            RegistteredMobileNumber = user.RegisteredMobileNo,
            RegistteredEmail = getMemberProfiles?.Email,
            IsMember =
                string.IsNullOrWhiteSpace(getMemberProfiles?.MemPk) || user.MEMPK == "-"
                    ? false
                    : true,
            StaffNo = getMemberProfiles?.StaffNo,
            IsStaff= string.IsNullOrWhiteSpace(getMemberProfiles?.StaffNo) || user.MEMPK == "-"
                    ? false
                    : true,
            ProfileImage = publicUrl
        };
        return result;
    }


}
