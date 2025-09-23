using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserAssignment.Queries.GetAvailableDriverVehicles;
public class AvailableDriverVehicleDto
{
    // Driver info
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string? Email { get; set; }

    // Vehicle info
    public Guid VehicleId { get; set; }
    public string LicensePlateNumber { get; set; } = default!;
    public string VehicleMake { get; set; } = default!;
    public string VehicleModel { get; set; } = default!;
    public string VehicleType { get; set; } = default!;
    public string VehicleOwner { get; set; } = default!;
}
