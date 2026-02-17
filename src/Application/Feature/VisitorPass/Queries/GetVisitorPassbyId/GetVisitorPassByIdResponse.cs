using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassbyId;

public class GetVisitorPassByIdResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public int VehicleLicenseNo { get; set; }
    public QuickPickType QuickPickType { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime ValidTill { get; set; }
    public string? QRCode { get; set; }
}
