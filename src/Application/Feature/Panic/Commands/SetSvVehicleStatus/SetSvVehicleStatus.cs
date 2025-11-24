using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.SetSvVehicleStatus;
public record SetSvVehicleStatusCommand(Guid Id, bool IsActive) : IRequest<Guid>;
public class SetSvVehicleStatusCommandHandler
    : IRequestHandler<SetSvVehicleStatusCommand,Guid>
{
    private readonly IApplicationDbContext _context;

    public SetSvVehicleStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(SetSvVehicleStatusCommand request, CancellationToken ct)
    {
        var entity = await _context.SvVehicles
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new NotFoundException("SvVehicle", request.Id.ToString());

        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }
}
