using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.SetSvPointStatus;
public record SetSvPointStatusCommand(Guid Id, bool IsActive) : IRequest<Guid>;
public class SetSvPointStatusCommandHandler
    : IRequestHandler<SetSvPointStatusCommand,Guid>
{
    private readonly IApplicationDbContext _context;

    public SetSvPointStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(SetSvPointStatusCommand request, CancellationToken ct)
    {
        var entity = await _context.SvPoints
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new NotFoundException("SvPoint", request.Id.ToString());

        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }
}

