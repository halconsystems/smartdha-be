using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.SubModules.Commands.DeleteSubModule;
public record DeleteSubModuleCommand(string Id) : IRequest<bool>;

public class DeleteSubModuleCommandHandler : IRequestHandler<DeleteSubModuleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteSubModuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteSubModuleCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.Id, out var guid))
        {
            throw new ArgumentException("Invalid GUID format", nameof(request.Id));
        }

        var submodule = await _context.SubModules.FindAsync(new object[] { guid }, cancellationToken);

        if (submodule == null)
        {
            throw new NotFoundException("Sub Module not found", request.Id);
        }

        submodule.IsDeleted = true;

       var result=  await _context.SaveChangesAsync(cancellationToken);


        return result > 0;
    }
}
