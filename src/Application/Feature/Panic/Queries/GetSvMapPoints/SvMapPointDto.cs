using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvMapPoints;

public class SvMapResponseDto
{
    public List<SvMapPointDto> Points { get; set; } = new();
    public List<SvMapVehicleDto> Vehicles { get; set; } = new();
}
public class SvMapPointDto
{
    public Guid PointId { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; } = default!;
}


public class SvMapVehicleDto
{
    public Guid VehicleId { get; set; }
    public string Name { get; set; } = default!;
    public string RegistrationNo { get; set; } = default!;
    public string VehicleType { get; set; } = default!;   // "Ambulance", "Bike" etc.
    public string? MapIconKey { get; set; }               // map icon
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public string Status { get; set; } = default!;        // Available / Busy / Offline
    public DateTime? LastLocationAt { get; set; }
    // 👇 ADD NEW FIELD
    public PanicInfoDto? PanicInfo { get; set; }
    // ✅ NEW
    public DriverInfoDto? Driver { get; set; }
}
public class PanicInfoDto
{
    public Guid PanicId { get; set; }
    public string CaseNo { get; set; } = default!;
    public PanicStatus Status { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string EmergencyName { get; set; } = default!;
    public DateTime AssignedAt { get; set; }
}

public class DriverInfoDto
{
    public string DriverId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public bool IsActive { get; set; }
}
