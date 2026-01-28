using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IRoleService
{
    Task<bool> RoleExistsAsync(Guid roleId, CancellationToken ct = default);

    Task<IList<Guid>> GetUserRoleIdsAsync(
        string userId,
        CancellationToken ct = default);

    // ✅ NEW
    Task<string?> GetRoleNameAsync(
        Guid roleId,
        CancellationToken ct = default);
}



