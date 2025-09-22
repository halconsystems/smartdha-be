using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_BowserAssignmentHistory : BaseAuditableEntity
{
    public Guid RequestId { get; set; }
    public OLH_BowserRequest Request { get; set; } = default!;
    public Guid? DriverId { get; set; }
    public OLH_DriverInfo? Driver { get; set; }
    public Guid? VehicleId { get; set; }
    public OLH_Vehicle? Vehicle { get; set; }

    public DateTime AssignedAt { get; set; }
    public string? AssignedBy { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
