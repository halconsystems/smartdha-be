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
    public DateOnly SlotDate { get; set; } // Full PKT DateTime
    [Required]
    public DateOnly SlotDateOnly { get; set; } // Just the date portion
    [Required]
    public TimeOnly FromTimeOnly { get; set; } // Just the time portion

    [Required]
    public TimeOnly ToTimeOnly { get; set; } // Just the time portion

    [Required]
    public AvailabilityAction Action { get; set; } = AvailabilityAction.Available;

    public string? Reason { get; set; } // e.g., “Maintenance”, “Admin Override”
}
