using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.RejectCaseWithRequirements;
public class RejectCaseRequest
{
    public string Remarks { get; set; } = default!;
    public List<RejectRequirementInput> Requirements { get; set; } = new();
}

