using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Queries.GetRoomCategories;
public record GetRoomCategoriesQuery() : IRequest<SuccessResponse<List<RoomCategoryDto>>>
{
    public ClubType ClubType { get; set; }
}

public class GetRoomCategoriesQueryHandler
    : IRequestHandler<GetRoomCategoriesQuery, SuccessResponse<List<RoomCategoryDto>>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public GetRoomCategoriesQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper)
    {
        _ctx = ctx;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<List<RoomCategoryDto>>> Handle(GetRoomCategoriesQuery request, CancellationToken ct)
    {
        var q = _ctx.RoomCategories
            .AsNoTracking();

        var list = await q
            .Where(x => x.IsDeleted == null || x.IsDeleted == false
                && x.Rooms.Any(r => r.Club.ClubType == request.ClubType))
            .OrderBy(x => x.Name)
            .ProjectTo<RoomCategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return new SuccessResponse<List<RoomCategoryDto>>(list);
        // or if you have a helper: return Success.Ok(list);
    }
}

