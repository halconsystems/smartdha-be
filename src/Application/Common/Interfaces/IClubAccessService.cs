using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IClubAccessService
{
    /// <summary>
    /// Returns:
    /// - null → SuperAdmin (access to all clubs)
    /// - list → restricted clubs
    /// Throws if user has no club access
    /// </summary>
    Task<List<Guid>?> GetAllowedClubIdsAsync(
        string userId,
        CancellationToken ct);
}

