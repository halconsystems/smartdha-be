using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public class PanicResponderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? Email { get; set; }
    public string? Gender { get; set; }
    public Guid EmergencyTypeId { get; set; }
    public string EmergencyTypeName { get; set; } = default!;
}
