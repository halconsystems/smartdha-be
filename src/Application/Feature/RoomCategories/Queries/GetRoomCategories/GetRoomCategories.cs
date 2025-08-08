using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Queries.GetRoomCategories;
public record GetRoomCategoriesQuery(
    bool IncludeInactive = false,
    int Page = 1,
    int PageSize = 50
) : IRequest<SuccessResponse<List<RoomCategoryDto>>>;

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
            .AsNoTracking()
            .Where(x => x.IsDeleted == false || x.IsDeleted == null);

        if (!request.IncludeInactive)
            q = q.Where(x => x.IsActive == true || x.IsActive == null);

        var list = await q
            .OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<RoomCategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return new SuccessResponse<List<RoomCategoryDto>>(list);
        // or if you have a helper: return Success.Ok(list);
    }
}

