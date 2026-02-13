using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.Roles.Queries.GetAssignableRoles;
public record GetAssignableRolesQuery(string CurrentUserId) : IRequest<List<string>>;

public class GetAssignableRolesHandler : IRequestHandler<GetAssignableRolesQuery, List<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public GetAssignableRolesHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<List<string>> Handle(GetAssignableRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.CurrentUserId);
        if (user == null) throw new UnauthorizedAccessException("Invalid user.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var assignableRoles = new List<string>();

        //foreach (var role in currentRoles)
        //{
        //    var childRoles = await _context.RoleAssignments
        //        .Where(x => x.ParentRole == role)
        //        .Select(x => x.ChildRole)
        //        .ToListAsync(cancellationToken);

        //    assignableRoles.AddRange(childRoles);
        //}

        return assignableRoles.Distinct().ToList();
    }
}
