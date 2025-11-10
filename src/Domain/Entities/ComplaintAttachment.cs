using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ComplaintAttachment : BaseAuditableEntity
{
    public string ComplaintId { get; set; } = default!;
    public Complaint Complaint { get; set; } = default!;
    public string ImageURL { get; set; } = default!;
    public string ImageExtension { get; set; } = default!;
    public string? ImageName { get; set; }
    public string? Description { get; set; }
}
