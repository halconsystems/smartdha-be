using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Queries.GetRoomCategories;
public record GetRoomCategoriesQuery() : IRequest<SuccessResponse<List<RoomCategoryDto>>>
{
    public Guid ClubId { get; set; }
    public ClubType ClubType { get; set; }
}

public class GetRoomCategoriesQueryHandler
    : IRequestHandler<GetRoomCategoriesQuery, SuccessResponse<List<RoomCategoryDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;

    public GetRoomCategoriesQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper, IApplicationDbContext appCtx, ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _mapper = mapper;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<List<RoomCategoryDto>>> Handle(GetRoomCategoriesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // Get user roles
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        //  If user is NOT superadmin → validate club assignment
        if (!isSuperAdmin)
        {
            bool isUserAssignedToClub = await _appCtx.UserClubAssignments
                .AnyAsync(uca => uca.UserId == userId && uca.ClubId == request.ClubId, ct);

            if (!isUserAssignedToClub)
                throw new UnAuthorizedException("You are not assigned to this club.");
        }

        var q = _ctx.RoomCategories
            .AsNoTracking()
            .Where(x =>
                (x.IsDeleted == null || x.IsDeleted == false) &&
                x.ClubType == request.ClubType &&
                x.ClubId == request.ClubId);

        var list = await q
            .OrderBy(x => x.Name)
            .ProjectTo<RoomCategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return new SuccessResponse<List<RoomCategoryDto>>(list);
        // or if you have a helper: return Success.Ok(list);
    }
}

