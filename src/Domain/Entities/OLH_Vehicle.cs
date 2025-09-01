using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_Vehicle:BaseAuditableEntity
{
    public Guid VehicleID { get; set; }

    public string LicensePlateNumber { get; set; } = default!;

    public string EngineNumber { get; set; } = default!;

    public string ChassisNumber { get; set; } = default!;

    public Guid MakeID { get; set; }

    public Guid ModelID { get; set; }

    public string YearofManufacture { get; set; } = default!;

    public Guid VehicleOwnerId { get; set; }

    public Guid VehicleTypeId { get; set; }

    public Guid VehicleStatusID { get; set; }

    public Guid BowserCapacityID { get; set; }

    public string Remarks { get; set; } = default!;

}
