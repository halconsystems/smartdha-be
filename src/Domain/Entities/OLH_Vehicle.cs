using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_Vehicle : BaseAuditableEntity
{
    public string LicensePlateNumber { get; set; } = default!;
    public string EngineNumber { get; set; } = default!;
    public string ChassisNumber { get; set; } = default!;
    public Guid? MakeId { get; set; }
    public OLH_VehicleMake? Make { get; set; }
    public Guid? ModelId { get; set; }
    public OLH_VehicleModel? Model { get; set; }
    public string YearOfManufacture { get; set; } = default!;
    public Guid VehicleOwnerId { get; set; }
    public OLH_VehicleOwner VehicleOwner { get; set; } = default!;
    public Guid VehicleTypeId { get; set; }
    public OLH_VehicleType VehicleType { get; set; } = default!;
    public Guid VehicleStatusId { get; set; }
    public OLH_VehicleStatus VehicleStatus { get; set; } = default!;
    public Guid BowserCapacityId { get; set; }
    public OLH_BowserCapacity BowserCapacity { get; set; } = default!; // dictates size
    public string? Remarks { get; set; }
}
