using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ComplaintAttachment : BaseAuditableEntity
{
    [Required]
    public Guid ComplaintId { get; set; }
    public Complaint Complaint { get; set; } = default!;

    [Required]
    public string ImageURL { get; set; } = default!;
    [Required]
    public string ImageExtension { get; set; } = default!;
    public string? ImageName { get; set; }
    public string? Description { get; set; }
}
