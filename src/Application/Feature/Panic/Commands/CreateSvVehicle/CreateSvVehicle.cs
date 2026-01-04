using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreateSvVehicle;
public record CreateSvVehicleCommand(
    string Name,
    string RegistrationNo,
    SvVehicleType VehicleType,
    string? MapIconKey,
    Guid? SvPointId,
    SvVehicleStatus Status
) : IRequest<Guid>;
public class CreateSvVehicleCommandHandler
    : IRequestHandler<CreateSvVehicleCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSvVehicleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSvVehicleCommand request, CancellationToken ct)
    {
        // Optional: verify point exists
        // Check only if a point was provided
        if (request.SvPointId.HasValue)
        {
            var pointExists = await _context.SvPoints
                .AnyAsync(p => p.Id == request.SvPointId.Value && p.IsActive==true, ct);

            if (!pointExists)
                throw new NotFoundException("SvPoint", request.SvPointId.Value.ToString());
        }


        var entity = new SvVehicle
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            RegistrationNo = request.RegistrationNo,
            VehicleType = request.VehicleType,
            MapIconKey = request.MapIconKey,
            SvPointId = request.SvPointId,         // ✔ nullable value
            Status = request.Status
        };

        _context.SvVehicles.Add(entity);
        await _context.SaveChangesAsync(ct);

        return entity.Id;
    }
}

