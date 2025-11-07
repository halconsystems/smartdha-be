using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Bowzer.Web.Queries;
public class BowserDto
{
    public Guid Id { get; set; }
    public string LicensePlateNumber { get; set; } = default!;
    public string EngineNumber { get; set; } = default!;
    public string ChassisNumber { get; set; } = default!;
    public Guid? MakeId { get; set; }
    public Guid? ModelId { get; set; }
    public string YearOfManufacture { get; set; } = default!;
    public string VehicleOwnerName { get; set; } = default!;
    public string VehicleTypeName { get; set; } = default!;
    public string VehicleStatusName { get; set; } = default!;
    public string BowserCapacityName { get; set; } = default!;
    public string BowserCapacityUnit { get; set; } = default!;
    public string? Remarks { get; set; }
}
