using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? UserId { get; set; } = string.Empty!;   // Who did the action (Identity UserId)
    public string? Email {  get; set; } = string.Empty;
    public string? CNIC {  get; set; } = string.Empty;

    [Required]
    public string Action { get; set; } = default!;   // e.g. "UserCreated", "LoginSuccess", "LoginFailed"

    public string? Description { get; set; }         // Optional details
    public string? IpAddress { get; set; }
    public string? Device { get; set; }
    public string? Browser { get; set; }
    public AppType AppType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

