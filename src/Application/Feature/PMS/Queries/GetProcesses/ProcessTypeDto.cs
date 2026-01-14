using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PMS.Queries.GetProcesses;
public class ProcessTypeDto
{
    public int ProcessID { get; set; }
    public string ProcessName { get; set; } = default!;
    public bool For_mobileApp { get; set; }
}
