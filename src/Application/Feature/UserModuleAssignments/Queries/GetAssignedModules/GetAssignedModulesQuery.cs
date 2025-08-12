using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.UserModuleAssignments.Queries.GetAssignedModules;
public class GetAssignedModulesQuery : IRequest<SuccessResponse<List<AssignedModuleDto>>>
{
    [Required]
    public string UserId { get; set; } = default!;
}
public class GetAssignedModulesQueryHandler : IRequestHandler<GetAssignedModulesQuery, SuccessResponse<List<AssignedModuleDto>>>
{
    private readonly IApplicationDbContext _ctx;

    public GetAssignedModulesQueryHandler(IApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<List<AssignedModuleDto>>> Handle(GetAssignedModulesQuery request, CancellationToken ct)
    {
        var assignments = await _ctx.UserModuleAssignments
            .Where(x => x.UserId == request.UserId && x.IsDeleted != true && x.IsActive==true)
            .Include(x => x.Module)
            .Select(x => new AssignedModuleDto
            {
                AssignmentId = x.Id,
                ModuleId = x.ModuleId,
                ModuleName = x.Module!.Name,
                ModuleDescription = x.Module.Description,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                Created = x.Created
            })
            .ToListAsync(ct);

        return Success.Ok(assignments, "Assigned modules retrieved.");
    }
}


