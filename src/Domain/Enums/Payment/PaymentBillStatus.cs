using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.Payment;
public enum PaymentBillStatus
{
    Generated = 1,
    PartiallyPaid = 2,
    Paid = 3,
    Cancelled = 4,
    Expired = 5
}

