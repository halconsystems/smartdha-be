using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class OLH_BowserRequestStatusHistory : BaseAuditableEntity
{
    public Guid RequestId { get; set; }
    public OLH_BowserRequest Request { get; set; } = default!;
    public BowserStatus FromStatus { get; set; }
    public BowserStatus ToStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public string? Reason { get; set; }
}

