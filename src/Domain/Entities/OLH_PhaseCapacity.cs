using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_PhaseCapacity : BaseAuditableEntity
{
    public Guid PhaseId { get; set; }
    public OLH_Phase Phase { get; set; } = default!;
    public Guid BowserCapacityId { get; set; }
    public OLH_BowserCapacity BowserCapacity { get; set; } = default!;

    // Optional: per (Phase×Capacity) base rate & validity
    public decimal? BaseRate { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow.Date;
    public DateTime? EffectiveTo { get; set; }
}
