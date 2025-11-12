using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ComplaintHistory : BaseAuditableEntity
{
    public Guid ComplaintId { get; set; }
    public Complaint Complaint { get; set; } = default!;
    public string Action { get; set; } = default!;                // “StatusChanged”, “Assigned”, “AttachmentAdded”, etc.
    public string? FromValue { get; set; }
    public string? ToValue { get; set; }
    public string ActorUserId { get; set; } = default!;
}
