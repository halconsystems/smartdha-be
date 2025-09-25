using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacities.Queries;
public class BowserPhaseCapacityDto
{
    public Guid Id { get; set; }
    public Guid PhaseId { get; set; }
    public Guid BowserCapacityId { get; set; }
    public decimal? BaseRate { get; set; }
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow.Date;
    public DateTime? EffectiveTo { get; set; }
}
