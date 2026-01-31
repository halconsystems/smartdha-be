using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class BanquetCharge : BaseAuditableEntity
{
    public Guid BookingId { get; set; }

    public string Title { get; set; } = default!;
    public decimal Amount { get; set; }
}

