using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserLoginAudit : BaseAuditableEntity
{
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public DateTime LoginAt { get; set; } = DateTime.Now;
    public bool IsSuccess { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceToken { get; set; }
    public string? DeviceName { get; set; }  // mobile model
    public string? DeviceOS { get; set; }    // Android 14 etc.
    public string? IPAddress { get; set; }
    public DateTime? LogoutAt { get; set; }
    }

