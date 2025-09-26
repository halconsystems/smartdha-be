using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserAssignment.Queries.GetAssignedDrivers;
public class AssignedDriverDto
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

    // Bowser Request info
    public string RequestNo { get; set; } = default!;
    public DateTime RequestDate { get; set; }
    public string Phase { get; set; } = default!;
    public string Ext { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string Address { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime? RequestedDeliveryDate { get; set; }
    public DateTime? PlannedDeliveryDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
}
