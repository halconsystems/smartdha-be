using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands;
public class SubmitCaseRequest
{
    // Case
    public Guid UserPropertyId { get; set; }
    public Guid ProcessId { get; set; }

    public string? ApplicantName { get; set; }
    public string? ApplicantCnic { get; set; }
    public string? ApplicantMobile { get; set; }
    public string? ApplicantRemarks { get; set; }

    // Prerequisite VALUES (JSON string)
    // Example: [{ "prerequisiteDefinitionId":"...", "valueText":"ABC Solar" }]
    public string PrerequisiteValuesJson { get; set; } = default!;

    // Documents (key = prerequisiteDefinitionId)
    public List<IFormFile>? Files { get; set; }
}
