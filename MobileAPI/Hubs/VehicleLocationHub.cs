using DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvVehicleLocation;
using Microsoft.AspNetCore.SignalR;

namespace MobileAPI.Hubs;

public class VehicleLocationHub : Hub
{
    private readonly IMediator _mediator;

    public VehicleLocationHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    // This replaces your PATCH API
    public async Task UpdateVehicleLocation(Guid vehicleId, double latitude, double longitude)
    {
        await _mediator.Send(new UpdateSvVehicleLocationCommand(
            vehicleId,
            latitude,
            longitude
        ));
    }
}

