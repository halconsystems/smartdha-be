using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CaseRejectRequirement : BaseAuditableEntity
{
    public Guid CaseId { get; set; }
    public PropertyCase Case { get; set; } = default!;

    // What officer is asking for
    [Required, MaxLength(150)]
    public string Name { get; set; } = default!;        // e.g. "Site Plan"

    [Required, MaxLength(50)]
    public string Code { get; set; } = default!;        // e.g. SITE_PLAN

    public PrerequisiteType Type { get; set; }          // FileUpload, Text, etc.

    // Validation (optional)
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? AllowedExtensions { get; set; }

    // Applicant action tracking
    public bool IsUploaded { get; set; } = false;
    public Guid? UploadedDocumentId { get; set; }

    // Officer guidance
    [MaxLength(500)]
    public string? Remarks { get; set; }
}

