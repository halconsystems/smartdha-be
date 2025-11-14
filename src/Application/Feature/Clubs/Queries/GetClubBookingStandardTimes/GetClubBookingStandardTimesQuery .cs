using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubBookingStandardTimes;
public class GetClubBookingStandardTimesQuery : IRequest<List<object>>
{
    public Guid? ClubId { get; set; }
    public ClubType ClubType { get; set; }
}

public class GetClubBookingStandardTimesQueryHandler : IRequestHandler<GetClubBookingStandardTimesQuery, List<object>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IApplicationDbContext _appCtx;      
    private readonly ICurrentUserService _currentUser;

    public GetClubBookingStandardTimesQueryHandler(IOLMRSApplicationDbContext context, IApplicationDbContext appCtx, ICurrentUserService currentUser)
    {
        _context = context;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<List<object>> Handle(GetClubBookingStandardTimesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // 1Get current roles
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        var query = _context.ClubBookingStandardTimes
            .Where(x => x.IsActive == true && x.IsDeleted == false && x.Club.ClubType == request.ClubType);

        if (!isSuperAdmin)
        {
            var userClubIds = await _appCtx.UserClubAssignments
                 .Where(uca => uca.UserId == userId)
                .Select(uca => uca.ClubId)
                .ToListAsync(cancellationToken);

            if (!userClubIds.Any())
                throw new UnAuthorizedException("User is not assigned to any club.");

            // Filter to user's clubs
            query = query.Where(x => userClubIds.Contains(x.ClubId));
        }

        if (request.ClubId.HasValue)
        {
            query = query.Where(x => x.ClubId == request.ClubId.Value);
        }

        var result = await query
            .Select(x => new
            {
                x.Id,
                x.ClubId,
                ClubName = x.Club.Name,
                x.CheckInTime,
                x.CheckOutTime
            })
            .ToListAsync(cancellationToken);

        if (!result.Any())
            throw new Ardalis.GuardClauses.NotFoundException(nameof(ClubBookingStandardTime), "No Club Booking Standard Times ");

        return result.Cast<object>().ToList(); 
    }
}
