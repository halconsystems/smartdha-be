using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class ProcessStep : BaseAuditableEntity
{
    // FK -> Process
    public Guid ProcessId { get; set; }
    public ServiceProcess Process { get; set; } = default!;

    // Step order: 1,2,3...
    public int StepNo { get; set; }

    // Example: "CC Review", "TP&BC Technical Review", "Finance Voucher"
    [Required, MaxLength(150)]
    public string Name { get; set; } = default!;

    // Which directorate handles this step
    public Guid DirectorateId { get; set; }
    public Directorate Directorate { get; set; } = default!;

    // If this step requires voucher generation (finance step usually)
    public bool RequiresVoucher { get; set; } = false;

    // If this step requires payment before moving forward
    public bool RequiresPaymentBeforeNext { get; set; } = false;

    // Optional SLA
    public int? SlaHours { get; set; }
}

