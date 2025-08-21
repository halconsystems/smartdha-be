using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;


namespace DHAFacilitationAPIs.Application.Feature.SubModules.Commands.AddSubModule;
public record AddSubModuleCommand : IRequest<SuccessResponse<Guid>>
{
    public string ModuleId { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool RequiresPermission { get; set; } = default!;
}

public class AddSubModuleCommandHandler: IRequestHandler<AddSubModuleCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;
    public AddSubModuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<SuccessResponse<Guid>> Handle(AddSubModuleCommand request, CancellationToken cancellationToken)
    {
        var module = new SubModule
        {
            Id = Guid.NewGuid(),
            Value= request.Value,
            DisplayName= request.DisplayName,
            Name = request.Name,
            Description = request.Description,
            ModuleId = Guid.Parse(request.ModuleId),
            RequiresPermission= request.RequiresPermission

        };

        _context.SubModules.Add(module);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<Guid>(module.Id, "Sub-Module created successfully!");
    }


}
