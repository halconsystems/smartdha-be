using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreateEmergencyType;

public class CreateEmergencyTypeCommand : IRequest<Guid>
{
    public int Code { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? HelplineNumber { get; set; }
}

public class CreateEmergencyTypeCommandHandler
    : IRequestHandler<CreateEmergencyTypeCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateEmergencyTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateEmergencyTypeCommand request, CancellationToken ct)
    {
        var existEmgergency = await _context.EmergencyTypes.
            Where(x => x.Name == request.Name ||
            x.Code == request.Code)
            .FirstOrDefaultAsync(ct);

        if (existEmgergency != null) throw new NotFoundException($"Emergency Type '{ request.Name}' OR '{request.Code}' already Exist");

        var entity = new EmergencyType
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            HelplineNumber = request.HelplineNumber,
            IsDeleted=false,
            IsActive=true
        };

        _context.EmergencyTypes.Add(entity);
        await _context.SaveChangesAsync(ct);

        return entity.Id;
    }
}
