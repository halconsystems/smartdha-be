using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ApprovalProcess.Queries.GetProcessStepsByProcessId;
public class ProcessStepDto
{
    public int ProcessStepID { get; set; }
    public int ProcessID { get; set; }
    public string? ProcessStep { get; set; }
    public string? Description { get; set; }
    public string? Remarks { get; set; }
    public int Sequence { get; set; }
}

