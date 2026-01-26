using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;

public class ClubProcessPrerequisite : BaseAuditableEntity
{
    public Guid ProcessId { get; set; }
    public ClubServiceProcess Process { get; set; } = default!;

    public Guid PrerequisiteDefinitionId { get; set; }
    public ClubPrerequisiteDefinitions PrerequisiteDefinition { get; set; } = default!;

    // Is it mandatory for this process?
    public bool IsRequired { get; set; } = true;

    // Optional: required at which step (0 = before submit)
    public int RequiredByStepNo { get; set; } = 0;
}
