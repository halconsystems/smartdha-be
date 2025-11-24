using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateSvVehicle;
public record UpdateSvVehicleCommand(
    Guid Id,
    string Name,
    string RegistrationNo,
    SvVehicleType VehicleType,
    string? MapIconKey,
    Guid SvPointId,
    string? DriverUserId,
    SvVehicleStatus Status
) : IRequest<Guid>;
public class UpdateSvVehicleCommandHandler: IRequestHandler<UpdateSvVehicleCommand,Guid>
{
    private readonly IApplicationDbContext _context;

    public UpdateSvVehicleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(UpdateSvVehicleCommand request, CancellationToken ct)
    {
        var entity = await _context.SvVehicles
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new NotFoundException("SvVehicle", request.Id.ToString());

        entity.Name = request.Name;
        entity.RegistrationNo = request.RegistrationNo;
        entity.VehicleType = request.VehicleType;
        entity.MapIconKey = request.MapIconKey;
        entity.SvPointId = request.SvPointId;
        entity.DriverUserId = request.DriverUserId;
        entity.Status = request.Status;

        await _context.SaveChangesAsync(ct);

        return entity.Id;
    }
}

