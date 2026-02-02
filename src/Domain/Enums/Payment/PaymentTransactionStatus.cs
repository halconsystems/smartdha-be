using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.Payment;
public enum PaymentTransactionStatus
{
    Initiated = 1,
    TokenIssued = 2,
    Redirected = 3,
    Success = 4,
    Failed = 5,
    Expired = 6
}
