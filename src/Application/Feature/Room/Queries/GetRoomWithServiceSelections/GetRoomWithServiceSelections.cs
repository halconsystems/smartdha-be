using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Announcements.Commands.AddAnnouncement;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Room.Queries.GetRoomWithServiceSelections;
public class GetRoomWithServiceSelectionsQuery : IRequest<SuccessResponse<RoomWithServicesDto>>
{
    public Guid RoomId { get; set; }
    public ServiceType ServiceType { get; set; }
}

public class GetRoomWithServiceSelectionsQueryHandler
    : IRequestHandler<GetRoomWithServiceSelectionsQuery, SuccessResponse<RoomWithServicesDto>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public GetRoomWithServiceSelectionsQueryHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<RoomWithServicesDto>> Handle(GetRoomWithServiceSelectionsQuery request, CancellationToken ct)
    {
        // Load the room (projected) with related names
        var room = await _ctx.Rooms
            .AsNoTracking()
            .Where(r => r.Id == request.RoomId)
            .Select(r => new RoomBriefDto
            {
                Id = r.Id,
                ClubId = r.ClubId,
                ClubName = r.Club.Name,                       // requires navigation; ensure included in model
                RoomCategoryId = r.RoomCategoryId,
                RoomCategoryName = r.RoomCategory.Name,       // ditto
                ResidenceTypeId = r.ResidenceTypeId,
                ResidenceTypeName = r.ResidenceType.Name,     // ditto
                No = r.No,
                Name = r.Name,
                Description = r.Description,
                IsGloballyAvailable = r.IsGloballyAvailable
            })
            .SingleOrDefaultAsync(ct);

        if (room is null)
            return new SuccessResponse<RoomWithServicesDto>(data: default!)
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = $"Room '{request.RoomId}' not found."
            };


        // Get mapped service ids for the room
        var mappedIds = (await _ctx.ServiceMappings
         .AsNoTracking()
          .Where(m => m.RoomId == request.RoomId)
          .Select(m => m.ServiceId)
          .ToListAsync(ct))
          .ToHashSet();


        // Get full service list with Selected flag
        var services = await _ctx.Services
            .AsNoTracking()
            .Where(s => s.ServiceType == request.ServiceType)
            .OrderBy(s => s.Name)
            .Select(s => new ServiceSelectionDto
            {
                ServiceId = s.Id,
                Name = s.Name,
                Selected = mappedIds.Contains(s.Id)
            })
            .ToListAsync(ct);

        var payload = new RoomWithServicesDto
        {
            Room = room,
            Services = services
        };

        return Success.Ok(payload);
    }
}

