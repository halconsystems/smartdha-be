using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_BowserCapacity:BaseAuditableEntity
{
    public decimal Capacity { get; set; }            // e.g., 9, 20
    public string Unit { get; set; } = "Gallon";     // Gallon/Litre (keep consistent)
    public ICollection<OLH_PhaseCapacity> Phases { get; set; } = new List<OLH_PhaseCapacity>();

}
