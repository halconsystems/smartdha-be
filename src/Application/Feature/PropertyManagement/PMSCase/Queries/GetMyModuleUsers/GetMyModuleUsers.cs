using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetMyModuleUsers;

public record ModuleUserDropdownDto(
    string Id,
    string DisplayName
);

public record GetModuleUsersQuery(Guid ModuleId)
    : IRequest<ApiResult<List<ModuleUserDropdownDto>>>;

public class GetModuleUsersHandler
    : IRequestHandler<GetModuleUsersQuery, ApiResult<List<ModuleUserDropdownDto>>>
{
    private readonly IApplicationDbContext _appDb;
    private readonly ICurrentUserService _current;

    public GetModuleUsersHandler(
        IApplicationDbContext appDb,
        ICurrentUserService current)
    {
        _appDb = appDb;
        _current = current;
    }

    public async Task<ApiResult<List<ModuleUserDropdownDto>>> Handle(
        GetModuleUsersQuery r,
        CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return ApiResult<List<ModuleUserDropdownDto>>.Fail("Unauthorized.");

        var userId = _current.UserId.ToString()!;

        // 1️⃣ Ensure CURRENT USER belongs to requested module
        var hasAccess = await _appDb.Set<UserModuleAssignment>()
            .AsNoTracking()
            .AnyAsync(x =>
                x.UserId == userId &&
                x.ModuleId == r.ModuleId &&
                x.IsActive==true &&
                x.IsDeleted !=true,
                ct);

        if (!hasAccess)
            return ApiResult<List<ModuleUserDropdownDto>>
                .Fail("You do not have access to this module.");

        // 2️⃣ Get USERS of that module
        var users = await (
            from uma in _appDb.Set<UserModuleAssignment>().AsNoTracking()
            join u in _appDb.Set<ApplicationUser>().AsNoTracking()
                on uma.UserId equals u.Id
            where uma.ModuleId == r.ModuleId
                  && uma.IsActive==true
                  && uma.IsDeleted !=true
                  && u.IsActive
                  && !u.IsDeleted
            select new
            {
                u.Id,
                u.Name,
                Roles = u.UserRoles.Select(r => r.Role.Name)
            }
        ).ToListAsync(ct);

        // 3️⃣ Build dropdown list
        var result = users
            .Select(x =>
            {
                var roleText = x.Roles.Any()
                    ? string.Join(", ", x.Roles)
                    : "User";

                return new ModuleUserDropdownDto(
                    x.Id,
                    $"{x.Name} ({roleText})"
                );
            })
            .OrderBy(x => x.DisplayName)
            .ToList();

        return ApiResult<List<ModuleUserDropdownDto>>.Ok(result);
    }
}



