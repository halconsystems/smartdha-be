using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Modules.Commands.AddModule;
using DHAFacilitationAPIs.Domain.Entities;


namespace DHAFacilitationAPIs.Application.Feature.SubModules.Commands.AddSubModule;
public record AddSubModuleCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public string ModuleId { get; set; } = default!;
    public string Description { get; set; } = default!;
   
}

public class AddSubModuleCommandHandler: IRequestHandler<AddSubModuleCommand,Guid>
{
    private readonly IApplicationDbContext _context;
    public AddSubModuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(AddSubModuleCommand request, CancellationToken cancellationToken)
    {
        var module = new SubModule
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ModuleId = Guid.Parse(request.ModuleId)

        };

        _context.SubModules.Add(module);
        await _context.SaveChangesAsync(cancellationToken);

        return module.Id;
    }


}
