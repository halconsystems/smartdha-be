using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public class PanicDto
{
    public record DashboardSummaryDto(
    int Total,
    int Open,                 // Created + Acknowledged + InProgress
    int Resolved,
    int Cancelled,
    int Last24h,
    int Today,
    double? AvgAckMinutes,    // average time Created -> Acknowledged
    double? AvgResolveMinutes // average time Created -> Resolved
);

}
