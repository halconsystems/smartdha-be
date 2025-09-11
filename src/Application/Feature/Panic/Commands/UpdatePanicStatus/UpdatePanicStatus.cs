using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdatePanicStatus;
public record UpdatePanicStatusCommand(Guid Id, PanicStatus NewStatus, string? AssignToUserId = null, string? Remarks = null) : IRequest<Unit>;

public class UpdatePanicStatusHandler : IRequestHandler<UpdatePanicStatusCommand, Unit>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;
    private readonly IPanicRealtime _realtime;

    public UpdatePanicStatusHandler(IApplicationDbContext ctx, ICurrentUserService current, IPanicRealtime realtime)
        => (_ctx, _current, _realtime) = (ctx, current, realtime);

    public async Task<Unit> Handle(UpdatePanicStatusCommand r, CancellationToken ct)
    {
        var e = await _ctx.PanicRequests.FirstOrDefaultAsync(x => x.Id == r.Id, ct)
            ?? throw new KeyNotFoundException("Panic not found.");

        var from = (PanicStatus)e.Status;
        e.Status = (PanicStatus)r.NewStatus; // if your domain enum namespace differs

        if (!string.IsNullOrWhiteSpace(r.AssignToUserId))
            e.AssignedToUserId = r.AssignToUserId;

        var now = DateTime.UtcNow;
        if (e.Status == PanicStatus.Acknowledged) e.AcknowledgedAt = now;
        if (e.Status == PanicStatus.Resolved) e.ResolvedAt = now;
        if (e.Status == PanicStatus.Cancelled) e.CancelledAt = now;

        _ctx.PanicActionLogs.Add(new Domain.Entities.PanicActionLog
        {
            PanicRequestId = e.Id,
            ActionByUserId = _current.UserId.ToString() ?? "system",
            Action = "STATUS",
            Remarks = r.Remarks,
            FromStatus = (PanicStatus)from,
            ToStatus = e.Status
        });

        await _ctx.SaveChangesAsync(ct);
       
        //await _realtime.PanicUpdatedAsync(e.Id);
        return Unit.Value;
    }
}
