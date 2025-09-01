using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.SubModules.Queries.GetSubModuleList;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.SubModules.Queries.SubModuleList;

public record SubModuleListQuery : IRequest<SuccessResponse<List<AllModulesDto>>>
{ public Guid? Id { get; set; } };

public class SubModuleListQueryHandler: IRequestHandler<SubModuleListQuery, SuccessResponse<List<AllModulesDto>>>
{
    private readonly IApplicationDbContext _context;

    public SubModuleListQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<SuccessResponse<List<AllModulesDto>>> Handle(SubModuleListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Modules
            .Include(m => m.SubModules)
                .ThenInclude(sm => sm.Permissions)
            .Where(m => m.IsDeleted == null || m.IsDeleted == false && m.AppType==AppType.Web)
            .AsQueryable();

        if (request.Id.HasValue)
        {
            query = query.Where(m => m.Id == request.Id);
        }

        var modules = await query
            .Select(m => new AllModulesDto
            {
                Id = m.Id,
                Value = m.Value,
                DisplayName = m.DisplayName,
                AllSubModules = m.SubModules
                    .Where(sm => sm.IsDeleted == null || sm.IsDeleted == false)
                    .Select(sm => new AllSubModulesDto
                    {
                        Id = sm.Id,
                        Value = sm.Value,
                        DisplayName = sm.DisplayName,
                        Permissions = sm.Permissions
                            .Select(p => new PermissionDto
                            {
                                Id = p.Id,
                                Value = p.Value,
                                DisplayName = p.DisplayName
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<AllModulesDto>>(modules, "Modules with sub-modules and permissions fetched successfully.", "Modules");
    }
}

