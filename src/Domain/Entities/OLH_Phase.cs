using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_Phase : BaseAuditableEntity
{
    public string Name { get; set; } = default!; // e.g., "DHA – Phase 1"
    public string? Description { get; set; }
    public ICollection<OLH_PhaseCapacity> AllowedCapacities { get; set; } = new List<OLH_PhaseCapacity>();
}
