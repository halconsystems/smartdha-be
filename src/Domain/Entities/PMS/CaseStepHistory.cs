using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CaseStepHistory : BaseAuditableEntity
{
    public Guid CaseId { get; set; }
    public PropertyCase Case { get; set; } = default!;

    public Guid StepId { get; set; }
    public ProcessStep Step { get; set; } = default!;

    public StepAction Action { get; set; }

    // Who performed action (simple: store string)
    [MaxLength(150)]
    public string? PerformedBy { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    // Next step info (for debugging and reporting)
    public Guid? NextStepId { get; set; }
    public int? NextStepNo { get; set; }
}
