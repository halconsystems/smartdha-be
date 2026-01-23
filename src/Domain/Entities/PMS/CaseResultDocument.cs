using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CaseResultDocument : BaseAuditableEntity
{
    [Required]
    public Guid CaseId { get; set; }
    public PropertyCase Case { get; set; } = default!;

    [Required, MaxLength(150)]
    public string DocumentType { get; set; } = default!;
    // e.g. "NOC", "ApprovalLetter"

    [Required, MaxLength(200)]
    public string FileName { get; set; } = default!;

    [Required, MaxLength(500)]
    public string FilePath { get; set; } = default!;
    // e.g. /uploads/cases/{caseId}/noc.pdf

    [MaxLength(50)]
    public string ContentType { get; set; } = "application/pdf";

    public long FileSize { get; set; }

    public bool IsFinal { get; set; } = true; // Final output doc
}

