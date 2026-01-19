using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands;
public record SubmitCaseResponse(Guid CaseId, string CaseNo);
