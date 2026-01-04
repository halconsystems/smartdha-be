using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.DriverProfile;
public class DriverProfileDto
{
    // USER INFO
    public string UserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public UserType UserType { get; set; }
    public string CNIC { get; set; } = "";

    // VEHICLE INFO
    public bool HasAssignedVehicle { get; set; }
    public Guid? VehicleId { get; set; }
    public string? VehicleName { get; set; }
    public string? RegistrationNo { get; set; }
    public string? VehicleType { get; set; }
    public string? VehicleStatus { get; set; }
    public string? MapIconKey { get; set; }

    // LIVE LOCATION
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public DateTime? LastLocationAt { get; set; }
}
