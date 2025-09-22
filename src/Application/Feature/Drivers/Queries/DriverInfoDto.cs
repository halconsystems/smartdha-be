using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Drivers.Queries;
public class DriverInfoDto
{
    public Guid Id { get; set; }
    public string DriverName { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string? Email { get; set; }
    public string Gender { get; set; } = default!;
    public string LicenceNo { get; set; } = default!;
    public Guid DriverStatusId { get; set; }
    public string DriverStatusName { get; set; } = default!;
    public DateTime StatusDate { get; set; }
}

