using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.DeleteEmergencyType;
public class DeleteEmergencyTypeCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}

public class DeleteEmergencyTypeCommandHandler
    : IRequestHandler<DeleteEmergencyTypeCommand,Guid>
{
    private readonly IApplicationDbContext _context;

    public DeleteEmergencyTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(DeleteEmergencyTypeCommand request, CancellationToken ct)
    {
        var entity = await _context.EmergencyTypes
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            throw new NotFoundException(nameof(EmergencyType), request.Id.ToString());

        // Soft Delete → mark inactive
        entity.IsActive = false;
        entity.LastModified = DateTime.Now;

        await _context.SaveChangesAsync(ct);

        return entity.Id;
    }

}

