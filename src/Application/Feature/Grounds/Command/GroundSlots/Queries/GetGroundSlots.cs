using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Room_Availability.Queries.GetRoomAvailabilities;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundSlots.Queries;

public record GroundAvailabilityDto(
    Guid Id,
    Guid GroundId,    
    string SlotName,
    string? SlotPrice,
    string? Code,
    GroundType GroundType,
    GroundCategory GroundCategory,
    string GroundName,
    DateTime FromDate,
    DateTime ToDate,
    AvailabilityAction Action,
    string? Reason    
);


public record GetGroundAvailabilitiesQuery(
    GroundType GroundType,
    GroundCategory GroundCategory,
    Guid? GroundId = null,
    Guid? ClubId = null,
    DateOnly? From = null,
    DateOnly? To = null,
    AvailabilityAction? Action = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<SuccessResponse<List<GroundAvailabilityDto>>>;
public class GetGroundAvailabilitiesQueryHandler
    : IRequestHandler<GetGroundAvailabilitiesQuery, SuccessResponse<List<GroundAvailabilityDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IApplicationDbContext _appCtx;         // Auth/Assignments DbContext
    private readonly ICurrentUserService _currentUser;

    public GetGroundAvailabilitiesQueryHandler(IOLMRSApplicationDbContext ctx, IApplicationDbContext appCtx, ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }

    public async Task<SuccessResponse<List<GroundAvailabilityDto>>> Handle(
     GetGroundAvailabilitiesQuery request, CancellationToken ct)
    {
        var ground = await _ctx.Grounds.FirstOrDefaultAsync(x => x.Id == request.GroundId && x.GroundType == request.GroundType && x.GroundCategory == request.GroundCategory);

        if (ground == null) throw new KeyNotFoundException("Ground not found.");
        var raQ = _ctx.GroundSlots
            .AsNoTracking()
            .Where(x => x.IsDeleted == false || x.IsDeleted == null && x.GroundId == request.GroundId);


        if (request.GroundId is Guid rid)
            raQ = raQ.Where(x => x.GroundId == rid);

        if (request.Action.HasValue)
            raQ = raQ.Where(x => x.Action == request.Action.Value);

        // Date overlap
        if (request.From.HasValue && request.To.HasValue)
        {
            raQ = raQ.Where(x => x.FromDateOnly <= request.From && x.ToDateOnly >= request.To);
        }
        else if (request.From.HasValue)
        {
            raQ = raQ.Where(x => x.ToDateOnly >= request.From);
        }
        else if (request.To.HasValue)
        {
            raQ = raQ.Where(x => x.FromDateOnly <= request.To);
        }

        // ⚠️ Projection سے پہلے OrderBy کریں (raw scalar fields پر)
        var rows = await raQ
            .Select(ra => new
            {
                ra.Id,
                ra.GroundId,
                GroundName = ground.Name,
                SlotName = ra.SlotName,
                SlotPrice = ra.SlotPrice,
                DisplayName = ra.DisplayName,
                Code = ra.Code,
                ra.FromDate,
                ra.ToDate,
                ra.Action,
                ra.Reason,
                GroundType = ground.GroundType,
                GroundCategory = ground.GroundCategory
            })
            .OrderByDescending(x => x.FromDate)
            .ThenBy(x => x.SlotPrice)
            .ToListAsync(ct);

        // اب in-memory DTO بنائیں (constructor-based)
        var list = rows.Select(x => new GroundAvailabilityDto(
            x.Id,
            x.GroundId,
            x.SlotName,
            x.SlotPrice,
            x.Code,
            x.GroundType,
            x.GroundCategory,
            x.GroundName,
            x.FromDate,
            x.ToDate,
            x.Action,
            x.Reason
        )).ToList();

        return new SuccessResponse<List<GroundAvailabilityDto>>(list, "Room availability loaded.");
    }

}

