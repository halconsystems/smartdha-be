using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class OrderSummary :BaseAuditableEntity
{
    public Guid OrderId { get; set; }
    public Orders? Orders { get; set; }

    public Guid ItemId { get; set; }
    public LaundryItems? LaundryItems { get; set; }
    public string? ItemCount { get; set; }
    public string? ItemPrice { get; set; }
    public string? TotalCountPrice { get; set; }
}
