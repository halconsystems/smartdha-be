using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum BowserStatus : short
{
    Draft = 10,
    Submitted = 20,
    Accepted = 30,
    Dispatched = 40,
    OnRoute = 50,
    Arrived = 60,
    Delivering = 70,
    Completed = 80,
    Cancelled = 90,
    Failed = 100
}
