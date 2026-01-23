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

    public Guid StepId { get; set; }              // external step
    public ProcessStep Step { get; set; } = default!;

    public string? FromUserId { get; set; }
    public string? ToUserId { get; set; }

    public CaseAction Action { get; set; }     // Submitted, ForwardInternal, ForwardExternal, Reject

    public string? Remarks { get; set; }
    public string? PerformedByUserId { get; set; }
}

