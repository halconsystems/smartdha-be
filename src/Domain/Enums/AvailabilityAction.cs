using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum AvailabilityAction
{
    Available = 1,      // explicitly available in the window
    Unavailable = 2     // explicitly blocked (maintenance, hold, etc.)
}
