using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityUnitDiscount : BaseAuditableEntity
{
    public Guid FacilityUnitId { get; set; }
    public FacilityUnit FacilityUnit { get; set; } = default!;

    public Guid DiscountId { get; set; }
    public Discount Discount { get; set; } = default!;
}

