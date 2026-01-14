using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CasePrerequisiteValue : BaseAuditableEntity
{
    public Guid CaseId { get; set; }
    public PropertyCase Case { get; set; } = default!;

    public Guid PrerequisiteDefinitionId { get; set; }
    public PrerequisiteDefinition PrerequisiteDefinition { get; set; } = default!;

    // For Text/Number/Date/Bool store in simple flexible fields
    [MaxLength(2000)]
    public string? ValueText { get; set; }

    public decimal? ValueNumber { get; set; }
    public DateTime? ValueDate { get; set; }
    public bool? ValueBool { get; set; }

    // For external verification (NADRA etc.)
    public bool? IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }

    [MaxLength(500)]
    public string? VerificationRef { get; set; }
}

