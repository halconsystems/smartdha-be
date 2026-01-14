using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PMS.Queries.GetProcessPrerequisites;
public class ProcessPrerequisitesDto
{
    public int ProcessPrerequisitesID { get; set; }
    public string ProcessPrerequisites { get; set; } = default!;
    public bool IsData { get; set; }
}


