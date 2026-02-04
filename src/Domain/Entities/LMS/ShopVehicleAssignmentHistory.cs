using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.LMS;

public class ShopVehicleAssignmentHistory : BaseAuditableEntity
{
    public Guid VehicleId { get; set; }
    public SvVehicle Vehicle { get; set; } = default!;

    public string DriverUserId { get; set; } = default!;
    public ApplicationUser Driver { get; set; } = default!;

    public DateTime AssignedAt { get; set; }
    public DateTime? UnassignedAt { get; set; }
}
