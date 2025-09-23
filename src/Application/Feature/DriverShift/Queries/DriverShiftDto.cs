using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.DriverShift.Queries;
public class DriverShiftDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehiclePlateNo { get; set; } = default!;
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = default!;
    public Guid ShiftId { get; set; }
    public string ShiftName { get; set; } = default!;
    public DateOnly DutyDate { get; set; }
    public bool? IsActive { get; set; }
}

