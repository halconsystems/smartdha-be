using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs.Dtos;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs;

public record GetClubMembersQuery(Guid ClubId) : IRequest<List<ClubMemberDto>>;

public class GetClubMembersQueryHandler : IRequestHandler<GetClubMembersQuery, List<ClubMemberDto>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetClubMembersQueryHandler(IOLMRSApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<ClubMemberDto>> Handle(GetClubMembersQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var activeMemberships = await _context.UserClubMemberships
                .Where(m =>
                    m.ClubId == request.ClubId &&
                    m.StartDate <= now &&
                    (m.EndDate == null || m.EndDate >= now) &&
                    m.IsDeleted == false && m.IsActive == true)
                .ToListAsync(cancellationToken);

        if (!activeMemberships.Any())
            throw new NotFoundException(nameof(UserClubMembership), $"No active members found for club ID {request.ClubId}");

        var userIds = activeMemberships.Select(m => m.UserId.ToString()).Distinct().ToList();

        var users = await _userManager.Users
                .Where(u =>
                    userIds.Contains(u.Id) &&
                    u.AppType == AppType.Mobile &&
                    u.UserType == UserType.Member &&
                    u.IsDeleted == false &&
                    u.IsActive == true)
                .ToListAsync(cancellationToken);

        var result = (from membership in activeMemberships
                      join user in users on membership.UserId.ToString() equals user.Id
                      select new ClubMemberDto
                      {
                          UserId = Guid.Parse(user.Id),
                          Name = user.Name,
                          CNIC = user.CNIC,
                          MEMPK = user.MEMPK,
                          MobileNo = user.MobileNo,
                          RegisteredMobileNo = user.RegisteredMobileNo,
                          RegisteredEmail = user.RegisteredEmail,
                          RegistrationNo = user.RegistrationNo
                      }).ToList();

        if (!result.Any())
            throw new NotFoundException(nameof(ApplicationUser), $"No active member details found for club ID {request.ClubId}");

        return result;

    }
}
