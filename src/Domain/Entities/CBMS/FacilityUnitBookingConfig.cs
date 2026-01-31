using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityUnitBookingConfig : BaseAuditableEntity
{
    public Guid FacilityUnitId { get; set; }
    public FacilityUnit FacilityUnit { get; set; } = default!;
    public BookingMode BookingMode { get; set; }
    public bool RequiresApproval { get; set; }
    // Slot-based
    public int? SlotDurationMinutes { get; set; }
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
    // Pricing
    public decimal BasePrice { get; set; }
    // Capacity
    public int MaxConcurrentBookings { get; set; } = 1;
}


