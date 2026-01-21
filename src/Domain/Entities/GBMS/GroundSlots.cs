using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.GBMS;

public class GroundSlots :BaseAuditableEntity
{
    public Guid GroundId { get; set; }
    public Grounds? Grounds { get; set; }
    [Required]
    public string SlotName { get; set; } = default!;
    [Required]
    public string DisplayName { get; set; } = default!;    
    public string? Code { get; set; }
    [Required]
    public string SlotPrice { get; set; } = default!;

    [Required]
    public DateTime FromDate { get; set; } // Full PKT DateTime
    [Required]
    public DateOnly FromDateOnly { get; set; } // Just the date portion
    [Required]
    public TimeOnly FromTimeOnly { get; set; } // Just the time portion

    [Required]
    public DateTime ToDate { get; set; } // Full PKT DateTime
    [Required]
    public DateOnly ToDateOnly { get; set; } // Just the date portion
    [Required]
    public TimeOnly ToTimeOnly { get; set; } // Just the time portion

    [Required]
    public AvailabilityAction Action { get; set; } = AvailabilityAction.Available;

    public string? Reason { get; set; } // e.g., “Maintenance”, “Admin Override”
}
