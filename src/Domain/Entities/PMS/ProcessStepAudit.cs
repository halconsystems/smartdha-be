using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class ProcessStepAudit : BaseAuditableEntity
{
    public Guid ProcessId { get; set; }
    public Guid StepId { get; set; }

    public int StepNo { get; set; }
    public string Name { get; set; } = default!;

    public Guid DirectorateId { get; set; }

    public bool RequiresVoucher { get; set; }
    public bool RequiresPaymentBeforeNext { get; set; }
    public int? SlaHours { get; set; }

    public string Action { get; set; } = "DELETE"; // DELETE / UPDATE / ADD

    public string? DeletedBy { get; set; }
    public DateTime DeletedOn { get; set; }
}

