using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class BookingDiscount : BaseAuditableEntity
{
    public Guid BookingId { get; set; }
    public string DiscountName { get; set; } = default!;
    public decimal Amount { get; set; }
}

