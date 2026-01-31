using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class BookingDateRange : BaseAuditableEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = default!;

    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
}

