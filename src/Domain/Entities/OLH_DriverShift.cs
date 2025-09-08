using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_DriverShift : BaseAuditableEntity
{
    public Guid VehicleId { get; set; }
    public OLH_Vehicle Vehicle { get; set; } = default!;
    public Guid DriverId { get; set; }
    public OLH_DriverInfo DriverInfo { get; set; } = default!;
    public Guid ShiftId { get; set; }
    public OLH_Shift Shift { get; set; } = default!;
    public DateOnly DutyDate { get; set; }
}
