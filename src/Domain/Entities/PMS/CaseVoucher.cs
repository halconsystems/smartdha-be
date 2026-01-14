using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CaseVoucher : BaseAuditableEntity
{
    public Guid CaseId { get; set; }
    public PropertyCase Case { get; set; } = default!;

    public VoucherStatus Status { get; set; } = VoucherStatus.Pending;

    [MaxLength(50)]
    public string? VoucherNo { get; set; }

    public decimal Amount { get; set; }

    public DateTime? GeneratedAt { get; set; }

    // Optional: generated at which step
    public Guid? GeneratedAtStepId { get; set; }
    public ProcessStep? GeneratedAtStep { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}

