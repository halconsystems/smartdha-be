using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetSvVehicleSummary;
public class SvVehicleSummaryDto
{
    public int TotalPoints { get; set; }
    public int TotalVehicles { get; set; }
    public int TotalAvailable { get; set; }
    public int TotalBusy { get; set; }
    public int TotalOffline { get; set; }
}

