using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.SubModules.Queries.GetSubModuleList;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.SubModules.Queries.SubModuleList;

public record SubModuleListQuery : IRequest<SuccessResponse<List<AllSubModulesDto>>>
{ public Guid? Id { get; set; } };

public class SubModuleListQueryHandler: IRequestHandler<SubModuleListQuery, SuccessResponse<List<AllSubModulesDto>>>
{
    private readonly IApplicationDbContext _context;

    public SubModuleListQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<AllSubModulesDto>>> Handle(SubModuleListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SubModules
            .Include(s => s.Permissions)   // ✅ include permissions
            .Where(x => x.IsDeleted == null || x.IsDeleted == false)
            .AsQueryable();

        if (request.Id.HasValue)
        {
            query = query.Where(s => s.Id == request.Id);
        }

        var subModules = await query
            .Select(s => new AllSubModulesDto
            {
                Id = s.Id,
                Value = s.Value,
                DisplayName = s.DisplayName,
                Name = s.Name,
                Description = s.Description,
                ModuleId = s.ModuleId,
                RequiresPermission = s.RequiresPermission,
                Permissions = s.Permissions.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Value = p.Value,
                    DisplayName = p.DisplayName
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<AllSubModulesDto>>(subModules, "Sub-modules fetched successfully.", "SubModules");
    }
}

