using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.CreateRoomCategory;
public record CreateRoomCategoryCommand(string Name, string? Description, ClubType ClubType, Guid ClubId)
    : IRequest<SuccessResponse<string>>;

public class CreateRoomCategoryCommandHandler : IRequestHandler<CreateRoomCategoryCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;
    public CreateRoomCategoryCommandHandler(IOLMRSApplicationDbContext ctx, IApplicationDbContext appCtx, ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<string>> Handle(CreateRoomCategoryCommand request, CancellationToken ct)
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
                .AnyAsync(uca => uca.UserId == userId && uca.ClubId == request.ClubId, ct);

            if (!assignedToClub)
                throw new UnAuthorizedException("You are not assigned to this club and cannot create residence types for it.");
        }

        var entity = new Domain.Entities.RoomCategory
        {
            Name = request.Name,
            Description = request.Description,
            ClubType = request.ClubType,
            ClubId = request.ClubId,
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.UtcNow
        };
        _ctx.RoomCategories.Add(entity);
        await _ctx.SaveChangesAsync(ct);
        return Success.Created(entity.Id.ToString());
    }
}

