using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.UpdateEmergencyType;
public class UpdateEmergencyTypeCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public int Code { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? HelplineNumber { get; set; }
}

public class UpdateEmergencyTypeCommandHandler: IRequestHandler<UpdateEmergencyTypeCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public UpdateEmergencyTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(UpdateEmergencyTypeCommand request, CancellationToken ct)
    {
        var entity = await _context.EmergencyTypes
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            throw new NotFoundException(nameof(EmergencyType), request.Id.ToString());

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.HelplineNumber = request.HelplineNumber;
        await _context.SaveChangesAsync(ct);

        return entity.Id;
    }
}

