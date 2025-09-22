using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_VehicleMake : BaseAuditableEntity
{
    public string MakeName { get; set; } = default!;

    // One make has many models
    public ICollection<OLH_VehicleModel> Models { get; set; } = new List<OLH_VehicleModel>();
}
