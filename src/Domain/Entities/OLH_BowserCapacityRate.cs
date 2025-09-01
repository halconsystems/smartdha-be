using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_BowserCapacityRate:BaseAuditableEntity
{
   // public Guid BowserCapacityRateID { get; set; }

    public Guid BowserCapacityId { get; set; }

    public OLH_BowserCapacity BowserCapacity { get; set; } = default!;

    public decimal Rate { get; set; }


}
