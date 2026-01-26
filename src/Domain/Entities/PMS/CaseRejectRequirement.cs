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

    // 🔗 LINK to definition (NO DUPLICATION)
    public Guid PrerequisiteDefinitionId { get; set; }
    public PrerequisiteDefinition PrerequisiteDefinition { get; set; } = default!;

    // Optional officer note (case-specific)
    [MaxLength(500)]
    public string? Remarks { get; set; }

    // Applicant upload tracking
    public bool IsUploaded { get; set; } = false;
    public Guid? UploadedDocumentId { get; set; }

}


