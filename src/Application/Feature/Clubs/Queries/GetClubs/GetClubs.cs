using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubs;
public record GetClubsQuery(bool IncludeInactive = false, int Page = 1, int PageSize = 50)
    : IRequest<IReadOnlyList<Club>>;

public class GetClubsQueryHandler : IRequestHandler<GetClubsQuery, IReadOnlyList<Club>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public GetClubsQueryHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<Club>> Handle(GetClubsQuery request, CancellationToken ct)
    {
        var q = _ctx.Clubs.AsNoTracking()
            .Where(x => x.IsDeleted == false || x.IsDeleted == null);

        if (!request.IncludeInactive)
            q = q.Where(x => x.IsActive == true || x.IsActive == null);

        return await q
            .OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);
    }
}

