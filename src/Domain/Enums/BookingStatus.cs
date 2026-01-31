using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum BookingStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Rejected = 4,
    PaymentPending = 5,
    Confirmed = 6,
    Cancelled = 7,
    Completed = 8,
    Provisional=9
}

