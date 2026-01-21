using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundStandardTime.Command;

public record GroundBookingStandardTimeDto(
    Guid GroundId,
    TimeOnly CheckInTime,
    TimeOnly CheckOutTime
);

public record CreateGroundBookingStandardTimeCommand() : IRequest<SuccessResponse<Guid>>
{
    public GroundBookingStandardTimeDto Dto { get; set; } = default!;
}

public class CreateGroundBookingStandardTimeCommandHandler : IRequestHandler<CreateGroundBookingStandardTimeCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;

    public CreateGroundBookingStandardTimeCommandHandler(IOLMRSApplicationDbContext ctx, IApplicationDbContext appCtx, ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<Guid>> Handle(CreateGroundBookingStandardTimeCommand request, CancellationToken ct)
    {
        try
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
                .AnyAsync(x => x.GroundId == request.Dto.GroundId && (x.IsDeleted == false || x.IsDeleted == null), ct);

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

            var entity = new Domain.Entities.GBMS.GroundStandtardTime
            {
                GroundId = request.Dto.GroundId,
                CheckInTime = request.Dto.CheckInTime,
                CheckOutTime = request.Dto.CheckOutTime,
                IsActive = true,
                IsDeleted = false,
                Created = DateTime.Now
            };

            await _ctx.GroundStandtardTimes.AddAsync(entity, ct);
            await _ctx.SaveChangesAsync(ct);

            return Success.Created(entity.Id);
        }
        catch (Exception ex)
        {
            
            throw new Exception(ex.Message);
        }
    }
}

