using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundSlots.Queries;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Queries;

public class GetGroundSlotsQuery : IRequest<SuccessResponse<List<GroundSlotsdto>>>
{
    [Required]
    public Guid GroundId { get; set; }
}
public class GetGroundSlotsQueryHandler : IRequestHandler<GetGroundSlotsQuery, SuccessResponse<List<GroundSlotsdto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public GetGroundSlotsQueryHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<List<GroundSlotsdto>>> Handle(GetGroundSlotsQuery request, CancellationToken ct)
    {
        var images = await _ctx.GroundSlots
            .Where(x => x.GroundId == request.GroundId && x.IsDeleted != true)
            .Select(x => new GroundSlotsdto
            {
                Id = x.Id,
                GroundId = x.GroundId,
                SlotName = x.SlotName,
                SlotPrice = x.SlotPrice,
                DisplayName = x.DisplayName,
                Code = x.Code,
                Action = x.Action,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                FromDateOnly = x.FromDateOnly,
                FromTimeOnly = x.FromTimeOnly,
                ToDateOnly = x.ToDateOnly,
                ToTimeOnly = x.FromTimeOnly,
            })
            .ToListAsync(ct);

        return Success.Ok(images, "Ground Slots fetched successfully.");
    }
}


