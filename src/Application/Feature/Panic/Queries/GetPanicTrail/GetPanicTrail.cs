using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicTrail;
public record GetPanicTrailQuery(Guid PanicRequestId) : IRequest<List<PanicTrailPointDto>>;

public class GetPanicTrailQueryHandler : IRequestHandler<GetPanicTrailQuery, List<PanicTrailPointDto>>
{
    private readonly IApplicationDbContext _ctx;
    public GetPanicTrailQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<List<PanicTrailPointDto>> Handle(GetPanicTrailQuery r, CancellationToken ct)
        => await _ctx.PanicLocationUpdates.AsNoTracking()
            .Where(x => x.PanicRequestId == r.PanicRequestId)
            .OrderBy(x => x.RecordedAt)
            .Select(x => new PanicTrailPointDto(x.RecordedAt, x.Latitude, x.Longitude, x.AccuracyMeters))
            .ToListAsync(ct);
}
