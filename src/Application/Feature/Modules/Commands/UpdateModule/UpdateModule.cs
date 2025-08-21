using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateModule;
public record UpdateModuleCommand : IRequest<SuccessResponse<Guid>>
{
   public string Id { get; set; }=default!;
    public string Value { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Remarks { get; set; } = default!;
}

public class UpdateModuleCommandHandler : IRequestHandler<UpdateModuleCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateModuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.Id, out var guid))
        {
            throw new ArgumentException("Invalid GUID format", nameof(request.Id));
        }

        var module = await _context.Modules.FindAsync(new object[] { guid }, cancellationToken);

        if (module == null)
        {
            throw new NotFoundException(nameof(Module), request.Name);
        }

        module.Value = request.Value;
        module.DisplayName=request.DisplayName;
        module.Name = request.Name;
        module.Description = request.Description;
        module.Remarks = request.Remarks;

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<Guid>(module.Id, "Module updated successfully!");
    }
}

