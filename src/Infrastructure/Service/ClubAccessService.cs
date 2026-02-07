using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class ClubAccessService : IClubAccessService
{
    private readonly IApplicationDbContext _appCtx;

    public ClubAccessService(IApplicationDbContext appCtx)
    {
        _appCtx = appCtx;
    }

    public async Task<List<Guid>?> GetAllowedClubIdsAsync(
        string userId,
        CancellationToken ct)
    {
        // 1️⃣ Get roles
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        // 2️⃣ SuperAdmin → unrestricted
        if (isSuperAdmin)
            return null;

        // 3️⃣ Restricted user → assigned clubs
        var clubIds = await _appCtx.UserClubAssignments
            .Where(x => x.UserId == userId)
            .Select(x => x.ClubId)
            .ToListAsync(ct);

        if (!clubIds.Any())
            throw new UnAuthorizedException(
                "User is not assigned to any club.");

        return clubIds;
    }
}

