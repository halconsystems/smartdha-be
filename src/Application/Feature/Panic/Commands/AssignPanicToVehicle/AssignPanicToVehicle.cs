using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.AssignPanicToVehicle;
public record AssignPanicToVehicleCommand(
    Guid PanicId,
    Guid SvVehicleId
) : IRequest<Guid>;
public class AssignPanicToVehicleCommandHandler: IRequestHandler<AssignPanicToVehicleCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser; // to get control room user id
    // wrapper around SignalR

    public AssignPanicToVehicleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(AssignPanicToVehicleCommand request, CancellationToken ct)
    {
        var panic = await _context.PanicRequests
            .FirstOrDefaultAsync(p => p.Id == request.PanicId, ct)
            ?? throw new NotFoundException("Panic", request.PanicId.ToString());

        var vehicle = await _context.SvVehicles
            .FirstOrDefaultAsync(v => v.Id == request.SvVehicleId, ct)
            ?? throw new NotFoundException("SvVehicle", request.SvVehicleId.ToString());

        if (vehicle.Status != SvVehicleStatus.Available)
            throw new InvalidOperationException("Vehicle is not available.");

        var dispatch = new PanicDispatch
        {
            PanicId = panic.Id,
            SvVehicleId = vehicle.Id,
            Status = PanicDispatchStatus.Assigned,
            AssignedAtUtc = DateTimeOffset.UtcNow,
            AssignedByUserId = _currentUser.UserId.ToString()!
        };

        _context.PanicDispatches.Add(dispatch);
        // Update vehicle status to Busy
        vehicle.Status = SvVehicleStatus.Busy;
        await _context.SaveChangesAsync(ct);

        // Notify driver mobile app via SignalR
       // await _panicNotificationService.NotifyPanicAssignedToVehicleAsync(dispatch, ct);

        return dispatch.Id;
    }
}

