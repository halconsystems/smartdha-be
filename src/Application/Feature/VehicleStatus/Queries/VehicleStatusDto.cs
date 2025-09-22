using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Queries;
public class VehicleStatusDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
    public string? Remarks { get; set; }
}

