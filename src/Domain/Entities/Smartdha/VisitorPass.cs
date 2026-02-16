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
    public required QuickPickType QuickPickType { get; set; } 
    public  DateTime FromDate { get; set; } 
    public DateTime  ToDate { get; set; } 
    public DateTime  ValidTill { get; set; } 
    public string?  QRCode { get; set; } 
}
