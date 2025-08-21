using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public GetRolesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _context.AppRoles
            .Where(r => r.IsDeleted == null || r.IsDeleted == false) // ✅ Only active roles
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<RoleDto>>(roles, "Roles fetched successfully", "Roles");
    }
}

