using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.DeleteModule;
public record DeleteModuleCommand(string Id): IRequest<bool>;


public class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteModuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.Id, out var guid))
        {
            throw new ArgumentException("Invalid GUID format", nameof(request.Id));
        }

        var module = await _context.Modules.FindAsync(new object[] { guid }, cancellationToken);

        if (module == null)
        {
            throw new NotFoundException(nameof(Module), request.Id);
        }

        module.IsDeleted = true;

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}

