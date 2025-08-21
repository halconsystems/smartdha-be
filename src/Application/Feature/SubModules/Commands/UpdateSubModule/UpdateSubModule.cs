using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.SubModules.Commands.UpdateSubModule;
public record UpdateSubModuleCommand : IRequest<SuccessResponse<Guid>>
{
    public string Id { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ModuleId { get; set; } = default!;

}

public class UpdateSubModuleCommandHandler : IRequestHandler<UpdateSubModuleCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateSubModuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateSubModuleCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.Id, out var guid))
        {
            throw new ArgumentException("Invalid GUID format", nameof(request.Id));
        }

        var submodule = await _context.SubModules.FindAsync(new object[] { guid }, cancellationToken);

        if (submodule == null)
        {
            throw new NotFoundException(nameof(SubModule), request.Name);
        }

        submodule.Value = request.Value;
        submodule.DisplayName = request.DisplayName;
        submodule.Name = request.Name;
        submodule.Description = request.Description;
        submodule.ModuleId =Guid.Parse(request.ModuleId);

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<Guid>(submodule.Id, "Sub-Module update successfully!");
    }
}

