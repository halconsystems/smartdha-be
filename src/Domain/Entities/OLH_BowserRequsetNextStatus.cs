using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_BowserRequsetNextStatus:BaseAuditableEntity
{
    public Guid StatusId { get; set; }

    public Guid NextStatusId { get; set; }

    public OLH_BowserRequestStatus BowserRequestStatus { get; set; } = default!;


}
