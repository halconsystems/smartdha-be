using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Queries.GetRoomCategories;
public record GetRoomCategoriesQuery(bool IncludeInactive = false, int Page = 1, int PageSize = 50)
    : IRequest<IReadOnlyList<RoomCategory>>;

public class GetRoomCategoriesQueryHandler : IRequestHandler<GetRoomCategoriesQuery, IReadOnlyList<RoomCategory>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public GetRoomCategoriesQueryHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<RoomCategory>> Handle(GetRoomCategoriesQuery request, CancellationToken ct)
    {
        var q = _ctx.RoomCategories.AsNoTracking().Where(x => x.IsDeleted == false || x.IsDeleted == null);
        if (!request.IncludeInactive) q = q.Where(x => x.IsActive == true || x.IsActive == null);

        return await q.OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);
    }
}
