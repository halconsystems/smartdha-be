using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetMyActivePanic;
public record GetMyActivePanicQuery : IRequest<List<PanicRequestDto>>;

public class GetMyActivePanicQueryHandler : IRequestHandler<GetMyActivePanicQuery, List<PanicRequestDto>>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;
    public GetMyActivePanicQueryHandler(IApplicationDbContext ctx, ICurrentUserService current)
        => (_ctx, _current) = (ctx, current);

    public async Task<List<PanicRequestDto>> Handle(GetMyActivePanicQuery r, CancellationToken ct)
    {
        var uid = _current.UserId.ToString() ?? throw new UnAuthorizedException("Not signed in.");
        return await _ctx.PanicRequests
            .AsNoTracking()
            .Include(x => x.EmergencyType)
            .Where(x => x.RequestedByUserId == uid && x.Status != PanicStatus.Resolved && x.Status != PanicStatus.Cancelled)
            .OrderByDescending(x => x.Created)
            .Select(x => new PanicRequestDto(
                x.Id, x.CaseNo, x.EmergencyType.Code, x.EmergencyType.Name,
                x.Latitude, x.Longitude, x.Status, x.Created))
            .ToListAsync(ct);
    }
}
