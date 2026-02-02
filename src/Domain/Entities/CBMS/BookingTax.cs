using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class BookingTax : BaseAuditableEntity
{
    public Guid BookingId { get; set; }
    public string TaxName { get; set; } = default!;
    public decimal Percentage { get; set; }
    public decimal Amount { get; set; }
}

