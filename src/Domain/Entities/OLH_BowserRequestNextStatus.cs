using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_BowserRequestNextStatus:BaseAuditableEntity
{
    public Guid StatusId { get; set; }
    public OLH_BowserRequestStatus Status { get; set; } = default!;
    public Guid NextStatusId { get; set; }
    public OLH_BowserRequestStatus NextStatus { get; set; } = default!;
}
