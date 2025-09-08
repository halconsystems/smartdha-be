using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicLocationUpdate;
public record CreatePanicLocationUpdateCommand(
    Guid PanicRequestId, decimal Latitude, decimal Longitude, float? AccuracyMeters
) : IRequest<Guid>;

public class CreatePanicLocationUpdateValidator : AbstractValidator<CreatePanicLocationUpdateCommand>
{
    public CreatePanicLocationUpdateValidator()
    {
        RuleFor(x => x.PanicRequestId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}

public class CreatePanicLocationUpdateHandler : IRequestHandler<CreatePanicLocationUpdateCommand, Guid>
{
    private readonly IApplicationDbContext _ctx;
    private readonly ICurrentUserService _current;
    private readonly IPanicRealtime _realtime;

    public CreatePanicLocationUpdateHandler(IApplicationDbContext ctx, ICurrentUserService current, IPanicRealtime realtime)
        => (_ctx, _current, _realtime) = (ctx, current, realtime);

    public async Task<Guid> Handle(CreatePanicLocationUpdateCommand r, CancellationToken ct)
    {
        var uid = _current.UserId.ToString() ?? throw new UnAuthorizedException("Not signed in.");

        var request = await _ctx.PanicRequests
            .FirstOrDefaultAsync(x => x.Id == r.PanicRequestId && x.RequestedByUserId == uid, ct)
            ?? throw new NotFoundException("Panic request not found or not owned by user.");

        var loc = new PanicLocationUpdate
        {
            PanicRequestId = r.PanicRequestId,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            AccuracyMeters = r.AccuracyMeters,
            RecordedAt = DateTime.Now
        };

        _ctx.PanicLocationUpdates.Add(loc);
        await _ctx.SaveChangesAsync(ct);

        await _realtime.LocationUpdatedAsync(request.Id, loc.Id);
        return loc.Id;
    }
}
