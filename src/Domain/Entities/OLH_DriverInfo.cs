using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_DriverInfo : BaseAuditableEntity
{
    public string DriverName { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string? Email { get; set; }
    public string Gender { get; set; } = default!;
    public string LicenceNo { get; set; } = default!;
    public Guid DriverStatusId { get; set; }
    public OLH_DriverStatus DriverStatus { get; set; } = default!;
    public DateTime StatusDate { get; set; }
}
