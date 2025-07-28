using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Roles.Queries.GetAllRoles;
public record GetAllRolesQuery : IRequest<List<string>>;

public class GetAllRolesHandler : IRequestHandler<GetAllRolesQuery, List<string>>
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IApplicationDbContext _context;

    public GetAllRolesHandler(RoleManager<IdentityRole> roleManager, IApplicationDbContext applicationDbContext)
    {
        _roleManager = roleManager;
        _context = applicationDbContext;
    }

    public async Task<List<string>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {

        
        var allWebRoles = AllRoles.GetWebRoles().ToList();

        var existingRoles = _roleManager.Roles
            .Where(r => allWebRoles.Contains(r.Name!))
            .Select(r => r.Name!)
            .ToList();

        return await Task.FromResult(existingRoles);
    }
}
