using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicLogs;
public record GetPanicLogsQuery(Guid PanicRequestId) : IRequest<List<PanicLogDto>>;

public class GetPanicLogsQueryHandler : IRequestHandler<GetPanicLogsQuery, List<PanicLogDto>>
{
    private readonly IApplicationDbContext _ctx;
    public GetPanicLogsQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<List<PanicLogDto>> Handle(GetPanicLogsQuery r, CancellationToken ct)
        => await _ctx.PanicActionLogs.AsNoTracking()
            .Where(x => x.PanicRequestId == r.PanicRequestId)
            .OrderByDescending(x => x.Created)
            .Select(x => new PanicLogDto(
                x.Created, x.ActionByUserId, x.Action, x.Remarks,
                (PanicStatus)x.FromStatus, (PanicStatus)x.ToStatus))
            .ToListAsync(ct);
}
