using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.PMS;
public enum VoucherStatus
{
    NotRequired = 0,
    Pending = 1,
    Generated = 2,
    Cancelled = 3,
    Paid = 4
}
