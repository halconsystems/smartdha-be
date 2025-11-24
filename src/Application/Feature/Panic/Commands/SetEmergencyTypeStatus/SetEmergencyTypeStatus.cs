using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.SetEmergencyTypeStatus;
public record SetEmergencyTypeStatusCommand(Guid Id, bool IsActive) : IRequest<Guid>;

public class SetEmergencyTypeStatusCommandHandler
    : IRequestHandler<SetEmergencyTypeStatusCommand,Guid>
{
    private readonly IApplicationDbContext _context;

    public SetEmergencyTypeStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(SetEmergencyTypeStatusCommand request, CancellationToken ct)
    {
        var entity = await _context.EmergencyTypes
            .FirstOrDefaultAsync(e => e.Id == request.Id, ct);

        if (entity == null)
            throw new NotFoundException(nameof(EmergencyType), request.Id.ToString());

        entity.IsActive = request.IsActive;
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }
}
