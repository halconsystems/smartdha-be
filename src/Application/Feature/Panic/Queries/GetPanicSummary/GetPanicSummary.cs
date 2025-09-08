using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicSummary;
public record GetPanicSummaryQuery(DateTime? From = null, DateTime? To = null) : IRequest<PanicSummaryDto>;

public class GetPanicSummaryQueryHandler : IRequestHandler<GetPanicSummaryQuery, PanicSummaryDto>
{
    private readonly IApplicationDbContext _ctx;
    public GetPanicSummaryQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<PanicSummaryDto> Handle(GetPanicSummaryQuery r, CancellationToken ct)
    {
        var q = _ctx.PanicRequests.AsNoTracking().Include(x => x.EmergencyType).AsQueryable();
        if (r.From is not null) q = q.Where(x => x.Created >= r.From);
        if (r.To is not null) q = q.Where(x => x.Created < r.To);

        var list = await q.ToListAsync(ct);

        int C(PanicStatus s) => list.Count(x => x.Status == s);
        var byType = list.GroupBy(x => x.EmergencyType.Name)
                         .Select(g => (g.Key, g.Count()))
                         .ToList();

        return new PanicSummaryDto(
            list.Count,
            C(PanicStatus.Created), C(PanicStatus.Acknowledged),
            C(PanicStatus.InProgress), C(PanicStatus.Resolved), C(PanicStatus.Cancelled),
            byType
        );
    }
}
