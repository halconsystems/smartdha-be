using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetNearestVehicles;
public class NearestVehicleDto
{
    public Guid VehicleId { get; set; }
    public string Name { get; set; } = default!;
    public string RegistrationNo { get; set; } = default!;
    public SvVehicleType VehicleType { get; set; }
    public string? MapIconKey { get; set; }
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public double? DistanceKm { get; set; } = default!;
    public double? EstimatedMinutes { get; set; } = default!;
}
