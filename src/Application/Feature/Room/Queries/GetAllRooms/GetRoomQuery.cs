using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;
public record GetRoomsQuery(Guid Id) : IRequest<IEnumerable<RoomDto>>;



public class GetRoomsQueryHandler : IRequestHandler<GetRoomsQuery, IEnumerable<RoomDto>>
{
    private readonly IOLMRSApplicationDbContext  _context;

    public GetRoomsQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoomDto>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var roomsQuery = _context.Rooms.AsQueryable();

        if (request.Id != Guid.Empty)
        {
            roomsQuery = roomsQuery.Where(r => r.Id == request.Id);
        }

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
                IsGloballyAvailable = room.IsGloballyAvailable
            }
        ).ToListAsync(cancellationToken);

        return roomDtos;
    }

}



