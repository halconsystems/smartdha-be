using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicPaged;
public record GetPanicPagedQuery(
    int Page = 1, int Size = 25,
    int? Code = null, PanicStatus? Status = null,
    DateTime? From = null, DateTime? To = null,
    string? Sort = null
) : IRequest<List<PanicRequestListDto>>;

public class GetPanicPagedQueryHandler : IRequestHandler<GetPanicPagedQuery, List<PanicRequestListDto>>
{
    private readonly IApplicationDbContext _ctx;
    public GetPanicPagedQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<List<PanicRequestListDto>> Handle(GetPanicPagedQuery r, CancellationToken ct)
    {
        var q = _ctx.PanicRequests.AsNoTracking().Include(x => x.EmergencyType).AsQueryable();

        if (r.Code is not null) q = q.Where(x => x.EmergencyType.Code == r.Code);
        if (r.Status is not null) q = q.Where(x => x.Status == r.Status);
        if (r.From is not null) q = q.Where(x => x.Created >= r.From);
        if (r.To is not null) q = q.Where(x => x.Created < r.To);

        q = r.Sort?.ToLowerInvariant() switch
        {
            "created_asc" => q.OrderBy(x => x.Created),
            "status_asc" => q.OrderBy(x => x.Status).ThenByDescending(x => x.Created),
            "status_desc" => q.OrderByDescending(x => x.Status).ThenByDescending(x => x.Created),
            _ => q.OrderByDescending(x => x.Created)
        };

        return await q.Skip((r.Page - 1) * r.Size).Take(r.Size)
            .Select(x => new PanicRequestListDto(
                x.Id, x.CaseNo, x.EmergencyType.Code, x.EmergencyType.Name,
                x.Latitude, x.Longitude, (PanicStatus)x.Status, x.Created))
            .ToListAsync(ct);
    }
}
