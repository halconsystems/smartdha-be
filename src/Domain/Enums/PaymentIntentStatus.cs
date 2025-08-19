using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums;
public enum PaymentIntentStatus { RequiresPayment = 1, Processing = 2, Succeeded = 3, Cancelled = 4, Expired = 5, Partial = 6 }
