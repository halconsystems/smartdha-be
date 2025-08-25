using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.SubModules.Commands.DeleteSubModule;
public record DeleteSubModuleCommand(string Id) : IRequest<SuccessResponse<bool>>;

public class DeleteSubModuleCommandHandler : IRequestHandler<DeleteSubModuleCommand, SuccessResponse<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteSubModuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<bool>> Handle(DeleteSubModuleCommand request, CancellationToken cancellationToken)
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


        return new SuccessResponse<bool>(
         result > 0,
         result > 0 ? "Sub-module deleted successfully!" : "Failed to delete sub-module"
     );
    }
}
