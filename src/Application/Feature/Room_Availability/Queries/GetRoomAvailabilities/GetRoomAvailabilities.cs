using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Room_Availability.Queries.GetRoomAvailabilities;
public record GetRoomAvailabilitiesQuery(
    Guid? RoomId = null,
    Guid? ClubId = null,
    DateTime? From = null,
    DateTime? To = null,
    AvailabilityAction? Action = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<SuccessResponse<List<RoomAvailabilityDto>>>;
public class GetRoomAvailabilitiesQueryHandler
    : IRequestHandler<GetRoomAvailabilitiesQuery, SuccessResponse<List<RoomAvailabilityDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public GetRoomAvailabilitiesQueryHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<List<RoomAvailabilityDto>>> Handle(
     GetRoomAvailabilitiesQuery request, CancellationToken ct)
    {
        var raQ = _ctx.RoomAvailabilities
            .AsNoTracking()
            .Where(x => x.IsDeleted == false || x.IsDeleted == null);

        if (request.RoomId is Guid rid)
            raQ = raQ.Where(x => x.RoomId == rid);

        if (request.Action.HasValue)
            raQ = raQ.Where(x => x.Action == request.Action.Value);

        // Date overlap
        if (request.From.HasValue && request.To.HasValue)
        {
            var from = request.From.Value;
            var to = request.To.Value;
            raQ = raQ.Where(x => x.FromDate < to && x.ToDate > from);
        }
        else if (request.From.HasValue)
        {
            var from = request.From.Value;
            raQ = raQ.Where(x => x.ToDate > from);
        }
        else if (request.To.HasValue)
        {
            var to = request.To.Value;
            raQ = raQ.Where(x => x.FromDate < to);
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

