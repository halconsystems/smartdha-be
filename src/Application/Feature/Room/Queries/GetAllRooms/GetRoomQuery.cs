using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;
public record GetRoomsQuery(Guid Id, ClubType ClubType) : IRequest<IEnumerable<RoomDto>>;

public class GetRoomsQueryHandler : IRequestHandler<GetRoomsQuery, IEnumerable<RoomDto>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;

    public GetRoomsQueryHandler(
         IOLMRSApplicationDbContext context,
         IApplicationDbContext appCtx,
         ICurrentUserService currentUser)
    {
        _context = context;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<RoomDto>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // 1️⃣ Check if user is SuperAdmin
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        // 2️⃣ Base query
        var roomsQuery = _context.Rooms
            .Where(r => r.Club.ClubType == request.ClubType)
            .AsQueryable();

        if (request.Id != Guid.Empty)
        {
            roomsQuery = roomsQuery.Where(r => r.Id == request.Id);
        }

        // 3️⃣ Restrict to assigned clubs if not SuperAdmin
        if (!isSuperAdmin)
        {
            var assignedClubIds = await _appCtx.UserClubAssignments
                .Where(uca => uca.UserId == userId)
                .Select(uca => uca.ClubId)
                .ToListAsync(cancellationToken);

            roomsQuery = roomsQuery.Where(r => assignedClubIds.Contains(r.ClubId));
        }

        var today = DateOnly.FromDateTime(DateTime.Now);

        // 4️⃣ Project to DTO
        var roomDtos = await (
            from room in roomsQuery
            join club in _context.Clubs on room.ClubId equals club.Id
            join category in _context.RoomCategories on room.RoomCategoryId equals category.Id
            join residence in _context.ResidenceTypes on room.ResidenceTypeId equals residence.Id
            select new RoomDto
            {
                Id = room.Id,
                ClubId = room.ClubId,
                ClubName = club.Name,
                RoomCategoryId = room.RoomCategoryId,
                RoomCategoryName = category.Name,
                ResidenceTypeId = room.ResidenceTypeId,
                ResidenceTypeName = residence.Name,
                No = room.No,
                Name = room.Name,
                Description = room.Description,
                IsGloballyAvailable = room.IsGloballyAvailable,
                NormalOccupancy = room.NormalOccupancy,
                MaxExtraOccupancy = room.MaxExtraOccupancy,

                Charges = _context.RoomCharges
                    .Where(rc => rc.RoomId == room.Id)
                    .Select(rc => new RoomChargeDto
                    {
                        BookingType = rc.BookingType,
                        ExtraOccupancy = rc.ExtraOccupancy,
                        Charges = rc.Charges
                    })
                    .ToList(),

                IsAvailableForApp = _context.RoomAvailabilities
                    .Any(a => a.RoomId == room.Id
                           && a.Action == AvailabilityAction.Available
                           && today < a.ToDateOnly),

                MainImageUrl = _context.RoomImages
                    .Where(img => img.RoomId == room.Id && img.Category == ImageCategory.Main)
                    .Select(img => img.ImageURL)
                    .FirstOrDefault()
            }
        ).ToListAsync(cancellationToken);

        return roomDtos;


        //var roomsQuery = _context.Rooms.AsQueryable();

        //if (request.Id != Guid.Empty)
        //{
        //    roomsQuery = roomsQuery.Where(r => r.Id == request.Id);
        //}
        //var today = DateOnly.FromDateTime(DateTime.Now);
        //var roomDtos = await (
        //    from room in roomsQuery
        //    join club in _context.Clubs on room.ClubId equals club.Id
        //    join category in _context.RoomCategories on room.RoomCategoryId equals category.Id
        //    join residence in _context.ResidenceTypes on room.ResidenceTypeId equals residence.Id
        //    select new RoomDto
        //    {
        //        Id = room.Id,
        //        ClubId = room.ClubId,
        //        ClubName = club.Name,
        //        RoomCategoryId = room.RoomCategoryId,
        //        RoomCategoryName = category.Name,
        //        ResidenceTypeId = room.ResidenceTypeId,
        //        ResidenceTypeName = residence.Name,
        //        No = room.No,
        //        Name = room.Name,
        //        Description = room.Description,
        //        IsGloballyAvailable = room.IsGloballyAvailable,
        //        NormalOccupancy = room.NormalOccupancy,
        //        MaxExtraOccupancy = room.MaxExtraOccupancy,

        //        // ✅ Load charges list
        //        Charges = _context.RoomCharges
        //            .Where(rc => rc.RoomId == room.Id)
        //            .Select(rc => new RoomChargeDto
        //            {
        //                BookingType = rc.BookingType,
        //                ExtraOccupancy = rc.ExtraOccupancy,
        //                Charges = rc.Charges
        //            })
        //            .ToList(),
        //        IsAvailableForApp = _context.RoomAvailabilities
        //    .Any(a => a.RoomId == room.Id
        //           && a.Action == AvailabilityAction.Available
        //           && today < a.ToDateOnly),
        //        MainImageUrl = _context.RoomImages
        //    .Where(img => img.RoomId == room.Id && img.Category == ImageCategory.Main)
        //    .Select(img => img.ImageURL)
        //    .FirstOrDefault()
        //    }
        //).ToListAsync(cancellationToken);

        //return roomDtos;
    }
}



