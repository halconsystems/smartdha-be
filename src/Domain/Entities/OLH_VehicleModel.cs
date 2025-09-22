using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_VehicleModel : BaseAuditableEntity
{
    public string ModelName { get; set; } = default!;

    // FK to Make
    public Guid MakeId { get; set; }
    public OLH_VehicleMake Make { get; set; } = default!;
}

