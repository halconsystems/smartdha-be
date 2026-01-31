using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class BookingSchedule : BaseAuditableEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = default!;

    // SlotBased
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

