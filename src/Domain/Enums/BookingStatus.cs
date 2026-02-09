using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum BookingStatus
{
    Pending = 0,      // Reserved, awaiting payment
    Confirmed = 1,    // Paid or approved
    Cancelled = 2,
    Expired = 3
}

