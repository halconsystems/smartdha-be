using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvMapPoints;
public class SvMapPointDto
{
    public Guid PointId { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set;} = default!;
    public List<SvMapVehicleDto> Vehicles { get; set; } = new();
}

public class SvMapVehicleDto
{
    public Guid VehicleId { get; set; }
    public string Name { get; set; } = default!;
    public string RegistrationNo { get; set; } = default!;
    public string VehicleType { get; set; } = default!;   // "Ambulance", "Bike" etc.
    public string? MapIconKey { get; set; }               // map icon
    public decimal? LastLatitude { get; set; }
    public decimal? LastLongitude { get; set; }
    public string Status { get; set; } = default!;        // Available / Busy / Offline
}

