using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicles;
public class SvVehicleListDto
{
    public Guid VehicleId { get; set; }
    public string Name { get; set; } = default!;
    public string RegistrationNo { get; set; } = default!;
    public string VehicleType { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? MapIconKey { get; set; }

    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public DateTime? LastLocationAt { get; set; }

    public Guid? PointId { get; set; }
    public string PointName { get; set; } = default!;
    public string? DriverUserId { get; set; }
    // Current Driver Assignment
    public bool? IsActive { get; set; }
    // NEW fields
    public bool IsDriverAssigned { get; set; }
    public string? AssignedDriverName { get; set; }
    public string? AssignedDriverPhone { get; set; }
    public string? AssignedDriverCnic { get; set; }
    public DateTime? AssignedAt { get; set; }
}

