using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.Smartdha;

public class LuggagePass : BaseAuditableEntity
{
    public required string Name { get; set; }
    public required string CNIC { get; set; }
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public int VehicleLicenseNo { get; set; }
    public string Description  { get; set; } = string.Empty;
    public  LuggagePassType? LuggagePassType { get; set; } 
    public DateTime? ValidTo { get; set; }
    public DateTime? ValidFrom { get; set; }
    public string? QRCode { get; set; }
    public string? TagId { get; set; }
    public string? PdfFilePath { get; set; }
}
