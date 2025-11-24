using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicleStatusSummary;
public class SvVehicleStatusSummaryDto
{
    public int Total { get; set; }
    public int Offline { get; set; }
    public int Available { get; set; }
    public int Busy { get; set; }
    public int Maintenance { get; set; }
}

