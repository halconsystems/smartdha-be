using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_PaymentStatus
{
    public string Status { get; set; } = default!; // Pending, Authorized, Captured, Refunded, Failed, Cancelled
}
