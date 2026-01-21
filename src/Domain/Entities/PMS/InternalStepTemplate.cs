using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class InternalStepTemplate : BaseAuditableEntity
{
    public Guid ProcessStepId { get; set; }     // link to external step
    public int StepNo { get; set; }            // 1..n internal
    public string Name { get; set; } = default!;

    // dynamic routing by role or designation instead of hard user
    public string AssignToRole { get; set; } = default!;  // "Admin", "Clerk", "Supdt"

    public bool IsFinalApprover { get; set; }   // Supdt = true (or Admin for smaller directorates)
    public int? SlaHours { get; set; }
}

