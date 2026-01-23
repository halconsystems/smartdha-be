using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries;
public class AdminCaseOwnerDto
{
    public string UserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string MobileNumber { get; set; } = default!;
}
