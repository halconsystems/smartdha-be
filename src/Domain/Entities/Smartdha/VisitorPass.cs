using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.Smartdha;
public class VisitorPass : BaseAuditableEntity
{
    public required string Name { get; set; }
    public required string CNIC { get; set; }
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public int VehicleLicenseNo { get; set; } 
    public required VisitorPassType VisitorPassType { get; set; } 
    public DateTime?  ValidTo { get; set; } 
    public DateTime?  ValidFrom { get; set; } 
    public string?  QRCode { get; set; } 
}
