using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class RoleService : IRoleService
{
    private readonly IApplicationDbContext _identityDb;

    public RoleService(IApplicationDbContext identityDb)
    {
        _identityDb = identityDb;
    }

    // 1️⃣ Check RoleId exists
    public async Task<bool> RoleExistsAsync(
        Guid roleId,
        CancellationToken ct = default)
    {
        return await _identityDb.Set<AppRole>()
            .AnyAsync(r => r.Id == roleId, ct);
    }

    // 2️⃣ Get RoleIds by UserId
    public async Task<IList<Guid>> GetUserRoleIdsAsync(
        string userId,
        CancellationToken ct = default)
    {
        return await _identityDb.Set<AppUserRole>()
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId)
            .ToListAsync(ct);
    }

    // ✅ NEW METHOD
    public async Task<string?> GetRoleNameAsync(
        Guid roleId,
        CancellationToken ct = default)
    {
        return await _identityDb.AppRoles
            .Where(r => r.Id == roleId
                        && (r.IsDeleted == null || r.IsDeleted == false))
            .Select(r => r.Name)
            .FirstOrDefaultAsync(ct);
    }
}



