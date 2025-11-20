using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetRoles;
public record GetRolesQuery : IRequest<SuccessResponse<List<RoleDto>>>;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, SuccessResponse<List<RoleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetRolesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
            throw new UnAuthorizedException("Invalid user context.");

        // Check if current user is SuperAdmin
        var currentRoles = await _context.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId.ToString())
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        bool isSuperAdmin = currentRoles.Contains("SuperAdministrator");

        var query = _context.AppRoles
            .Where(r => (r.IsDeleted == null || r.IsDeleted == false) // Only active roles
                        && r.Name != "SuperAdministrator");           // Exclude SuperAdmin role itself

        if (!isSuperAdmin)
        {
            // Normal users can only see roles they created
            query = query.Where(r => r.CreatedBy == userId.ToString());
        }

        var roles = await query
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync(cancellationToken);

        //var roles = await _context.AppRoles
        //    .Where(r => (r.IsDeleted == null || r.IsDeleted == false)   // ✅ Only active roles
        //                && r.Name != "SuperAdministrator"               // ✅ Exclude SuperAdmin
        //                && r.CreatedBy == userId.ToString())               // ✅ Only current user created
        //    .Select(r => new RoleDto
        //    {
        //        Id = r.Id,
        //        Name = r.Name
        //    })
        //    .ToListAsync(cancellationToken);

        return new SuccessResponse<List<RoleDto>>(roles, "Roles fetched successfully", "Roles");
    }
}


