using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.AddPanicNote;
public record AddPanicNoteCommand(Guid Id, string Note) : IRequest<Unit>;

public class AddPanicNoteCommandHandler : IRequestHandler<AddPanicNoteCommand, Unit>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;
    private readonly IPanicRealtime _realtime;

    public AddPanicNoteCommandHandler(IApplicationDbContext ctx, ICurrentUserService current, IPanicRealtime realtime)
        => (_ctx, _current, _realtime) = (ctx, current, realtime);

    public async Task<Unit> Handle(AddPanicNoteCommand r, CancellationToken ct)
    {
        var e = await _ctx.PanicRequests.FirstOrDefaultAsync(x => x.Id == r.Id, ct)
            ?? throw new KeyNotFoundException("Panic not found.");

        e.Notes = string.IsNullOrWhiteSpace(e.Notes) ? r.Note : $"{e.Notes}\n{r.Note}";

        _ctx.PanicActionLogs.Add(new Domain.Entities.PanicActionLog
        {
            PanicRequestId = e.Id,
            ActionByUserId = _current.UserId.ToString() ?? "system",
            Action = "NOTE",
            Remarks = r.Note,
            FromStatus = e.Status,
            ToStatus = e.Status
        });

        await _ctx.SaveChangesAsync(ct);
        //await _realtime.PanicUpdatedAsync(e.Id);
        return Unit.Value;
    }
}
