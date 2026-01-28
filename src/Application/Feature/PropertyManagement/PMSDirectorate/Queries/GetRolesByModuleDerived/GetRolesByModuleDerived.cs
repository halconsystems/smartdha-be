using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetRolesByModuleDerived;
public record GetRolesByModuleDerivedQuery(Guid ModuleId): IRequest<List<RoleModuleAccessDto>>;

public class GetRolesByModuleDerivedQueryHandler
    : IRequestHandler<GetRolesByModuleDerivedQuery, List<RoleModuleAccessDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRolesByModuleDerivedQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleModuleAccessDto>> Handle(
    GetRolesByModuleDerivedQuery request,
    CancellationToken cancellationToken)
    {
        var result = await (
       from um in _context.UserModuleAssignments.IgnoreQueryFilters()
       join ur in _context.AppUserRoles.IgnoreQueryFilters()
           on um.UserId equals ur.UserId
       join r in _context.AppRoles.IgnoreQueryFilters()
           on ur.RoleId equals r.Id
       where um.ModuleId == request.ModuleId
       select new RoleModuleAccessDto
       {
           RoleId = r.Id,
           RoleName = r.Name!,
           IsAssigned = true
       }
   )
   .Distinct()
   .OrderBy(x => x.RoleName)
   .ToListAsync(cancellationToken);

        return result;
    }

}
