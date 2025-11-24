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

    public decimal? LastLatitude { get; set; }
    public decimal? LastLongitude { get; set; }
    public DateTimeOffset? LastLocationAtUtc { get; set; }

    public Guid PointId { get; set; }
    public string PointName { get; set; } = default!;

    public string? DriverUserId { get; set; }
    public bool? IsActive { get; set; }
}

