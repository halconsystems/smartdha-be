using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvVehicleLocation;
public record UpdateSvVehicleLocationCommand(
    Guid VehicleId,
    double Latitude,
    double Longitude
) : IRequest<Guid>;
public class UpdateSvVehicleLocationCommandHandler
    : IRequestHandler<UpdateSvVehicleLocationCommand, Guid>
{
    private readonly IVehicleLocationStore _fileService;
    private readonly IPanicRealtime _realtime;

    public UpdateSvVehicleLocationCommandHandler(
        IVehicleLocationStore fileService,
        IPanicRealtime realtime)
    {
        _fileService = fileService;
        _realtime = realtime;
    }

    public async Task<Guid> Handle(UpdateSvVehicleLocationCommand request, CancellationToken ct)
    {
        DateTime lastUpdate = DateTime.Now;
        await _fileService.SaveLocationAsync(request.VehicleId, request.Latitude, request.Longitude, lastUpdate);

        // Real-time update
        await _realtime.UpdateLocationAsync(new Common.Models.UpdateLocation
        {
            VehicleId = request.VehicleId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            LastLocationUpdateAt = lastUpdate
        });

        return request.VehicleId;
    }
}

