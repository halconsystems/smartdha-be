using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum DriverStatus
{
    Available = 1,
    NotAvailable = 2,
    OnDuty = 3,
    OffDuty = 4,
    Suspended = 5
}
