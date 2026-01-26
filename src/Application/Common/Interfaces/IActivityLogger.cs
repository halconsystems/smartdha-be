using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IActivityLogger
{
    Task LogAsync(
        string action,
        string? userId = null,
        string? email = null,
        string? cnic = null,
        string? description = null,
        AppType appType = AppType.Web,
        CancellationToken ct = default);

    Task DeviceAsync(
        Guid UserId,
        string? DeviceId = null,
        string? FCMToken = null,
        CancellationToken ct = default);
}


