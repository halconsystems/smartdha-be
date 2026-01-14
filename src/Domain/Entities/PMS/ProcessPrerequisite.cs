using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class ProcessPrerequisite : BaseAuditableEntity
{
    public Guid ProcessId { get; set; }
    public ServiceProcess Process { get; set; } = default!;

    public Guid PrerequisiteDefinitionId { get; set; }
    public PrerequisiteDefinition PrerequisiteDefinition { get; set; } = default!;

    // Is it mandatory for this process?
    public bool IsRequired { get; set; } = true;

    // Optional: required at which step (0 = before submit)
    public int RequiredByStepNo { get; set; } = 0;
}

