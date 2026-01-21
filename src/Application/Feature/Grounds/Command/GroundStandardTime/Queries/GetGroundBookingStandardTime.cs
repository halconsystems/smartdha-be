using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundStandardTime.Queries;

public record GetGroundBookingStandardTimeQuery(Guid GroundId) : IRequest<List<object>>;


public class GetGroundBookingStandardTimeQueryHandler : IRequestHandler<GetGroundBookingStandardTimeQuery, List<object>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;

    public GetGroundBookingStandardTimeQueryHandler(IOLMRSApplicationDbContext context, IApplicationDbContext appCtx, ICurrentUserService currentUser)
    {
        _context = context;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<List<object>> Handle(GetGroundBookingStandardTimeQuery request, CancellationToken cancellationToken)
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

        var query = _context.GroundStandtardTimes
            .Where(x => x.IsActive == true && x.IsDeleted == false && x.GroundId == request.GroundId);

        var ground = await _context.Grounds.FirstOrDefaultAsync(x => x.Id == request.GroundId);
        if(ground == null)
            throw new KeyNotFoundException("Ground not Found.");

        var result = await query
            .Select(x => new
            {
                x.Id,
                x.GroundId,
                GroundName = ground.Name,
                x.CheckInTime,
                x.CheckOutTime
            })
            .ToListAsync(cancellationToken);

        if (!result.Any())
            throw new Ardalis.GuardClauses.NotFoundException(nameof(GroundStandtardTime), "No Ground Booking Standard Times ");

        return result.Cast<object>().ToList();
    }
}
