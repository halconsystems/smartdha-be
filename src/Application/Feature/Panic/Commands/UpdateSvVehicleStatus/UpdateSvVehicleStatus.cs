using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvVehicleStatus;
public record UpdateSvVehicleStatusCommand(
    Guid VehicleId,
    SvVehicleStatus Status
) : IRequest<Guid>;
public class UpdateSvVehicleStatusCommandHandler
    : IRequestHandler<UpdateSvVehicleStatusCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public UpdateSvVehicleStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(UpdateSvVehicleStatusCommand request, CancellationToken ct)
    {
        var entity = await _context.SvVehicles
            .FirstOrDefaultAsync(x => x.Id == request.VehicleId, ct);

        if (entity == null)
            throw new NotFoundException(nameof(SvVehicle), request.VehicleId.ToString());

        entity.Status = request.Status;

        // Optional: reset location when set to Offline
        // if (request.Status == SvVehicleStatus.Offline)
        // {
        //     entity.LastLatitude = null;
        //     entity.LastLongitude = null;
        // }

        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }
}

