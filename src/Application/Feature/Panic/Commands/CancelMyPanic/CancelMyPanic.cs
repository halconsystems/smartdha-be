using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CancelMyPanic;
public record CancelMyPanicCommand(Guid PanicRequestId, string? Remarks) : IRequest<Unit>;

public class CancelMyPanicCommandHandler
    : IRequestHandler<CancelMyPanicCommand, Unit>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;
    private readonly IPanicRealtime _realtime;

    public CancelMyPanicCommandHandler(
        IApplicationDbContext ctx,
        ICurrentUserService current,
        IPanicRealtime realtime)
        => (_ctx, _current, _realtime) = (ctx, current, realtime);

    public async Task<Unit> Handle(CancelMyPanicCommand r, CancellationToken ct)
    {
        var uid = _current.UserId.ToString() ?? throw new UnAuthorizedException("Not signed in.");

        var e = await _ctx.PanicRequests
            .FirstOrDefaultAsync(x => x.Id == r.PanicRequestId && x.RequestedByUserId == uid, ct)
            ?? throw new NotFoundException("Panic request not found.");

        if (e.Status is PanicStatus.Resolved or PanicStatus.Cancelled)
            return Unit.Value;

        var from = e.Status;
        e.Status = PanicStatus.Cancelled;
        e.CancelledAt = DateTime.Now;

        _ctx.PanicActionLogs.Add(new PanicActionLog
        {
            PanicRequestId = e.Id,
            ActionByUserId = uid,
            Action = "CANCEL",
            Remarks = r.Remarks,
            FromStatus = from,
            ToStatus = e.Status
        });

        await _ctx.SaveChangesAsync(ct);
       // await _realtime.PanicUpdatedAsync(e.Id);

        return Unit.Value;
    }
}
