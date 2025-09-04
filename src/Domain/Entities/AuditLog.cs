using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string UserId { get; set; } = default!;   // who did the action

    [Required]
    public string Action { get; set; } = default!;   // e.g. "CreateUser", "UpdateUser", "DeleteUser"

    public string EntityName { get; set; } = default!;   // e.g. "ApplicationUser"
    public string EntityId { get; set; } = default!;     // e.g. UserId, ModuleId, etc.

    public string? OldValues { get; set; }               // JSON string of previous values
    public string? NewValues { get; set; }               // JSON string of new values

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? IPAddress { get; set; }
    public string? Device { get; set; }
}

