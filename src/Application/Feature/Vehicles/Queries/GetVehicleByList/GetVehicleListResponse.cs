using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Queries.GetVehicleByList;

public class GetVehicleListResponse
{
    public int LicenseNo { get; set; }
    public string License { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? ETagId { get; set; }
    public DateTime? ValidTo { get; set; }
    public DateTime? ValidFrom { get; set; }
}
