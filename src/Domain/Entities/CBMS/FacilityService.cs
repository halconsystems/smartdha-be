using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityService : BaseAuditableEntity
{
    public Guid FacilityId { get; set; }
    public Facility Facility { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public decimal Price { get; set; }

    public bool IsQuantityBased { get; set; } // food per head
    public bool IsOptional { get; set; } = true;
}

