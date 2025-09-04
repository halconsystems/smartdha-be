using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_BowserCapacity:BaseAuditableEntity
{
   // public Guid BowserCapacityID { get; set; }

    public decimal Capacity { get; set; } = default!;

    public string Unit { get; set; } = default!;



}
