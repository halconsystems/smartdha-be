using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundStandardTime.Command;

public record GroundStandardTimeDto(
    Guid GroundId,
    TimeOnly CheckInTime,
    TimeOnly CheckOutTime
);

public record UpdateGroundStandardTimeCommand() : IRequest<SuccessResponse<Guid>>
{
    public GroundBookingStandardTimeDto Dto { get; set; } = default!;
}
public class UpdateGroundStandardTimeCommandHandler
    : IRequestHandler<UpdateGroundStandardTimeCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _appCtx;
    public UpdateGroundStandardTimeCommandHandler(IOLMRSApplicationDbContext ctx, ICurrentUserService currentUser, IApplicationDbContext appCtx)
    {
        _ctx = ctx;
        _currentUser = currentUser;
        _appCtx = appCtx;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateGroundStandardTimeCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        //  Get current roles
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        //  If user is NOT superadmin → validate club assignment
        if (!isSuperAdmin)
        {
            throw new UnAuthorizedException("You are not assigned to this Ground and cannot create residence types for it.");

        }

        // ensure no duplicate per club
        var exists = await _ctx.GroundStandtardTimes
            .FirstOrDefaultAsync(x => x.GroundId == request.Dto.GroundId && (x.IsDeleted == false || x.IsDeleted == null), ct);
        if(exists == null)
            throw new KeyNotFoundException("Ground Standard Time not Found.");

        exists.CheckInTime = request.Dto.CheckInTime;
        exists.CheckOutTime = request.Dto.CheckOutTime;

        await _ctx.SaveChangesAsync(ct);

        return Success.Created(exists.Id);
    }
}


