using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetPanicById;
public record GetPanicByIdQuery(Guid Id) : IRequest<PanicDetailDto>;

public class GetPanicByIdQueryHandler : IRequestHandler<GetPanicByIdQuery, PanicDetailDto>
{
    private readonly IApplicationDbContext _ctx;
    public GetPanicByIdQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;

    public async Task<PanicDetailDto> Handle(GetPanicByIdQuery r, CancellationToken ct)
    {
        var e = await _ctx.PanicRequests.AsNoTracking()
            .Include(x => x.EmergencyType)
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct)
            ?? throw new KeyNotFoundException("Panic not found.");

        return new PanicDetailDto(
            e.Id, e.CaseNo, e.EmergencyType.Code, e.EmergencyType.Name,
            e.Latitude, e.Longitude, (PanicStatus)e.Status,
            e.Created, e.AcknowledgedAt, e.ResolvedAt, e.CancelledAt,
            e.RequestedByUserId, e.AssignedToUserId, e.Notes, e.MediaUrl
        );
    }
}
