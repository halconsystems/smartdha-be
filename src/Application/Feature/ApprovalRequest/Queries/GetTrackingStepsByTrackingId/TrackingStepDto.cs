using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Queries.GetTrackingStepsByTrackingId;
public class TrackingStepDto
{
    public string? ProcessName { get; set; }
    public long TrackingId { get; set; }
    public string? StepName { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Sequence { get; set; }
    public string? Remarks { get; set; }
    public DateTime? ProcessDateTime { get; set; }
}

