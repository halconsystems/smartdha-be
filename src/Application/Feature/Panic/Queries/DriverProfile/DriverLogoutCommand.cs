using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.DriverProfile;

public record DriverLogoutCommand : IRequest<bool>;

public class DriverLogoutCommandHandler : IRequestHandler<DriverLogoutCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DriverLogoutCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DriverLogoutCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();

        // 1. Clear device token in UserLoginAudits
        var lastLogin = await _context.UserLoginAudits
            .Where(x => x.UserId == userId && x.IsSuccess)
            .OrderByDescending(x => x.LoginAt)
            .FirstOrDefaultAsync(ct);

        if (lastLogin != null)
        {
            lastLogin.DeviceToken = null;
            lastLogin.LogoutAt = DateTime.Now;
        }

        // 2. Find the driver's assigned vehicle
        var vehicle = await _context.SvVehicles
            .Where(v => v.DriverUserId == userId)
            .FirstOrDefaultAsync(ct);

        if (vehicle != null)
        {
            // ✔ Unassign the driver
            vehicle.DriverUserId = null;

            // ✔ Set status to Available (1)
            vehicle.Status = SvVehicleStatus.Available;

            // ✔ Optionally clear last location timestamp
            // vehicle.LastLocationAt = null;
        }

        // 3. Save all changes
        await _context.SaveChangesAsync(ct);

        return true;
    }
}

