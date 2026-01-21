using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.GBMS;

public class GroundBookingSlot :BaseAuditableEntity
{
    public Guid BookingId   { get; set; }
    public GroundBooking? GroundBooking { get; set; }
    public Guid SlotId { get; set; }
    public GroundSlots? GroundSlots { get; set; }    
    public string? SlotPrice { get; set; }

}
