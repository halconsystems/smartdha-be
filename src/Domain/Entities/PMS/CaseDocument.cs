using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CaseDocument : BaseAuditableEntity
{
    public Guid CaseId { get; set; }
    public PropertyCase Case { get; set; } = default!;
    // Optional link to a prerequisite (CNIC front, site plan)
    public Guid? PrerequisiteDefinitionId { get; set; }
    public PrerequisiteDefinition? PrerequisiteDefinition { get; set; }

    [Required, MaxLength(255)]
    public string FileName { get; set; } = default!;

    [Required, MaxLength(500)]
    public string FileUrl { get; set; } = default!; // S3/Azure/local storage path

    [MaxLength(50)]
    public string? ContentType { get; set; } // "application/pdf"

    public long? FileSize { get; set; }

    // NEW: null = original submission, NOT NULL = rejection upload
    public Guid? CaseRejectRequirementId { get; set; }
    public CaseRejectRequirement? CaseRejectRequirement { get; set; }
}

