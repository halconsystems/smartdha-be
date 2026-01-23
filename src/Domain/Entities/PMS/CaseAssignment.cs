using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CaseAssignment : BaseAuditableEntity
{
    public Guid CaseId { get; set; }

    public Guid DirectorateId { get; set; }     // current directorate (external step)
    public int InternalStepNo { get; set; }    // current internal stage
    public string Status { get; set; } = "Open"; // Open, Claimed, Completed
    public string? AssignedToUserId { get; set; }     // optional (when marked)
    public string? ClaimedByUserId { get; set; }      // if you support "claim"
    public DateTime? ClaimedAt { get; set; }
}

