using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Room_Availability.Queries.GetRoomAvailabilities;
public record GetRoomAvailabilitiesQuery(
    ClubType ClubType,
    Guid? RoomId = null,
    Guid? ClubId = null,
    DateOnly? From = null,
    DateOnly? To = null,
    AvailabilityAction? Action = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<SuccessResponse<List<RoomAvailabilityDto>>>;
public class GetRoomAvailabilitiesQueryHandler
    : IRequestHandler<GetRoomAvailabilitiesQuery, SuccessResponse<List<RoomAvailabilityDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IApplicationDbContext _appCtx;         // Auth/Assignments DbContext
    private readonly ICurrentUserService _currentUser;

    public GetRoomAvailabilitiesQueryHandler(IOLMRSApplicationDbContext ctx, IApplicationDbContext appCtx, ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<List<RoomAvailabilityDto>>> Handle(
     GetRoomAvailabilitiesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // get user roles
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        var raQ = _ctx.RoomAvailabilities
            .AsNoTracking()
            .Where(x => x.IsDeleted == false || x.IsDeleted == null)
            .Where(x => x.Room.Club.ClubType == request.ClubType);

        // 3Restrict to assigned clubs if not superadmin
        if (!isSuperAdmin)
        {
            var assignedClubIds = await _appCtx.UserClubAssignments
                .Where(uca => uca.UserId == userId)
                .Select(uca => uca.ClubId)
                .ToListAsync(ct);

            raQ = raQ.Where(x => assignedClubIds.Contains(x.Room.ClubId));
        }

        if (request.RoomId is Guid rid)
            raQ = raQ.Where(x => x.RoomId == rid);

        if (request.Action.HasValue)
            raQ = raQ.Where(x => x.Action == request.Action.Value);

        // Date overlap
        if (request.From.HasValue && request.To.HasValue)
        {
            var from = request.From.Value;
            var to = request.To.Value;
            raQ = raQ.Where(x => x.FromDateOnly <= from && x.ToDateOnly >= to);
        }
        else if (request.From.HasValue)
        {
            var from = request.From.Value;
            raQ = raQ.Where(x => x.ToDateOnly >= from);
        }
        else if (request.To.HasValue)
        {
            var to = request.To.Value;
            raQ = raQ.Where(x => x.FromDateOnly <= to);
        }

        // ⚠️ Projection سے پہلے OrderBy کریں (raw scalar fields پر)
        var rows = await raQ
            .Select(ra => new
            {
                ra.Id,
                ra.RoomId,
                RoomNo = ra.Room.No,
                RoomName = ra.Room.Name,
                ClubId = ra.Room.ClubId,
                ClubName = ra.Room.Club.Name,
                RoomCategoryId = ra.Room.RoomCategoryId,
                RoomCategoryName = ra.Room.RoomCategory.Name, // اگر آپ کے model میں Name ہے تو .Name کریں
                ResidenceTypeId = ra.Room.ResidenceTypeId,
                ResidenceTypeName = ra.Room.ResidenceType.Name,
                ra.FromDate,
                ra.ToDate,
                ra.Action,
                ra.Reason,
                IsGloballyAvailable = ra.Room.IsGloballyAvailable
            })
            .OrderByDescending(x => x.FromDate)
            .ThenBy(x => x.RoomNo)
            .ToListAsync(ct);

        // اب in-memory DTO بنائیں (constructor-based)
        var list = rows.Select(x => new RoomAvailabilityDto(
            x.Id,
            x.RoomId,
            x.RoomNo,
            x.RoomName,
            x.ClubId,
            x.ClubName,
            x.RoomCategoryId,
            x.RoomCategoryName,
            x.ResidenceTypeId,
            x.ResidenceTypeName,
            x.FromDate,
            x.ToDate,
            x.Action,
            x.Reason,
            x.IsGloballyAvailable
        )).ToList();

        return new SuccessResponse<List<RoomAvailabilityDto>>(list, "Room availability loaded.");
    }

}

