using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
[Table("RoomAvailabilities")]
public class RoomAvailability : BaseAuditableEntity
{
    [Required]
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;

    [Required]
    public DateTime FromDate { get; set; } // PKT stored directly

    [Required]
    public DateTime ToDate { get; set; }

    [Required]
    public AvailabilityAction Action { get; set; } = AvailabilityAction.Available;

    public string? Reason { get; set; }          // e.g., “Maintenance”, “Admin Override”
}
