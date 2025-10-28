using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_DriverStatus : BaseAuditableEntity
{
    public DriverStatus Status { get; set; } = DriverStatus.Available; // Available/NotAvailable/OnDuty/OffDuty/Suspended
}
