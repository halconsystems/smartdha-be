using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.BowserMake.Queries;
public class VehicleStatusDto
{
    public Guid Id { get; set; }
    public VehicleStatus Status { get; set; } = default!;
    public string? Remarks { get; set; }
}

