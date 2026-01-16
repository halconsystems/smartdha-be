using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class ServiceProcess : BaseAuditableEntity
{
    // FK -> Category
    public Guid CategoryId { get; set; }
    public ServiceCategory Category { get; set; } = default!;

    // Example: "Solar Panel Installation"
    [Required, MaxLength(150)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(30)]
    public string Code { get; set; } = default!;

    // If user must pay immediately on submit
    public bool IsFeeRequired { get; set; } = false;       // fee exists for this process
    public bool IsFeeAtSubmission { get; set; } = false;   // if true → fee required before submit

    // If finance voucher can be generated at some step
    public bool IsVoucherPossible { get; set; } = false;
    public bool IsNadraVerificationRequired { get; set; } = false;
    public bool IsfeeSubmit { get; set; } = false;
    public bool IsInstructionAtStart { get; set; } = false;
    public string? Instruction { get; set; } = default!;
}

