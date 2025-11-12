using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class Complaint : BaseAuditableEntity
{
    public string ComplaintNo { get; set; } = default!;          // e.g., CMP-2025-000123
    public string Title { get; set; } = default!;
    public string Notes { get; set; } = default!;
    public string CategoryCode { get; set; } = default!;         // FK -> Lookup
    public string PriorityCode { get; set; } = default!;         // FK -> Lookup
    public ComplaintStatus Status { get; set; } = ComplaintStatus.New;

    public string CreatedByUserId { get; set; } = default!;
    public string? AssignedToUserId { get; set; }

    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public DateTimeOffset? AcknowledgedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public string? AdminRemarks { get; set; } = default!;

    public ICollection<ComplaintAttachment> Attachments { get; set; } = [];
    public ICollection<ComplaintComment> Comments { get; set; } = [];
    public ICollection<ComplaintHistory> History { get; set; } = [];
}

