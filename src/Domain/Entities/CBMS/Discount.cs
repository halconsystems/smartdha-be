using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class Discount : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public DiscountType Type { get; set; }
    public decimal Value { get; set; }  // % or flat amount

    // Optional rules
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public decimal? MinOrderAmount { get; set; }
}
