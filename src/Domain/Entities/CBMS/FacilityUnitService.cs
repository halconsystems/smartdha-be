using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityUnitService : BaseAuditableEntity
{
    public Guid FacilityUnitId { get; set; }
    public FacilityUnit FacilityUnit { get; set; } = default!;
    public Guid FacilityServiceId { get; set; }
    public FacilityService FacilityService { get; set; } = default!;
    public decimal? OverridePrice { get; set; }
    public bool IsEnabled { get; set; } = true;
}

