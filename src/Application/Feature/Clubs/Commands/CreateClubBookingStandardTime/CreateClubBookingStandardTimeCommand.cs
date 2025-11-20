using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
public record CreateClubBookingStandardTimeCommand() : IRequest<SuccessResponse<Guid>>
{
    public ClubBookingStandardTimeDto Dto { get; set; } = default!;
}

public class CreateClubBookingStandardTimeCommandHandler : IRequestHandler<CreateClubBookingStandardTimeCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;

    public CreateClubBookingStandardTimeCommandHandler(IOLMRSApplicationDbContext ctx, IApplicationDbContext appCtx, ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<Guid>> Handle(CreateClubBookingStandardTimeCommand request, CancellationToken ct)
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
            bool assignedToClub = await _appCtx.UserClubAssignments
                .AnyAsync(uca => uca.UserId == userId && uca.ClubId == request.Dto.ClubId, ct);

            if (!assignedToClub)
                throw new UnAuthorizedException("You are not assigned to this club and cannot create residence types for it.");
        }

        // ensure no duplicate per club
        var exists = await _ctx.ClubBookingStandardTimes
            .AnyAsync(x => x.ClubId == request.Dto.ClubId && (x.IsDeleted == false || x.IsDeleted == null), ct);

        if (exists)
        {
            return new SuccessResponse<Guid>(
                Guid.Empty,
                "Club booking standard time already exists for this club."
            )
            {
                Status = StatusCodes.Status409Conflict
            };
        }

        var entity = new ClubBookingStandardTime
        {
            ClubId = request.Dto.ClubId,
            CheckInTime = request.Dto.CheckInTime,
            CheckOutTime = request.Dto.CheckOutTime,
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.Now
        };

        await _ctx.ClubBookingStandardTimes.AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id);
    }
}
