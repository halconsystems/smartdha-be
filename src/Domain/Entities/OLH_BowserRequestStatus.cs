using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_BowserRequestStatus:BaseAuditableEntity
{
    public string Status { get; set; } = default!;

    public int OrderBy { get; set; }
}
