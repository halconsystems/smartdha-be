using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public record PanicSummaryDto(
    int Total, int Created, int Acknowledged, int InProgress, int Resolved, int Cancelled,
    IReadOnlyList<(string Type, int Count)> ByType);
