using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Complaints.Web;
public class ComplaintDto
{
    public Guid Id { get; set; }
    public string ComplaintNo { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Notes { get; set; } = default!;
    public string CategoryCode { get; set; } = default!;
    public string PriorityCode { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string CreatedByUserId { get; set; } = default!;
    public string? AssignedToUserId { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public DateTimeOffset? AcknowledgedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public DateTime Created { get; set; }

    public List<ComplaintAttachmentDto> Attachments { get; set; } = [];
    public List<ComplaintCommentDto> Comments { get; set; } = [];
    public List<ComplaintHistoryDto> History { get; set; } = [];
}

public class ComplaintAttachmentDto
{
    public Guid Id { get; set; }
    public string ImageURL { get; set; } = default!;
    public string ImageExtension { get; set; } = default!;
    public string? ImageName { get; set; }
    public string? Description { get; set; }
}

public class ComplaintCommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = default!;
    public string Visibility { get; set; } = default!;
    public string CreatedByUserId { get; set; } = default!;
    public DateTime Created { get; set; }
}

public class ComplaintHistoryDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = default!;
    public string? FromValue { get; set; }
    public string? ToValue { get; set; }
    public string ActorUserId { get; set; } = default!;
    public DateTime Created { get; set; }
}
