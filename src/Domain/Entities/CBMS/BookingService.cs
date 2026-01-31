using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class BookingService : BaseAuditableEntity
{
    public Guid BookingId { get; set; }
    public Guid FacilityServiceId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

