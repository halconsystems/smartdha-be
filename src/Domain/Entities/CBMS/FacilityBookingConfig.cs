using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityBookingConfig : BaseAuditableEntity
{
    public Guid FacilityId { get; set; }
    public Facility Facility { get; set; } = default!;

    public BookingMode BookingMode { get; set; }

    public bool RequiresApproval { get; set; }

    public int? SlotDurationMinutes { get; set; } // 60 for Padel
    public int? MaxBookingsPerDay { get; set; }

    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
}

