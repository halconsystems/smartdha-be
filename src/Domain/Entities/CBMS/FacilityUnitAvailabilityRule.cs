using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityUnitAvailabilityRule : BaseAuditableEntity
{
    public Guid FacilityUnitId { get; set; }
    public FacilityUnit FacilityUnit { get; set; } = default!;

    // If null → applies to all dates
    public DateOnly? Date { get; set; }

    // Slot-based only (Padel)
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public bool IsAvailable { get; set; }   // true = allowed, false = blocked

    public string? Reason { get; set; }
}

