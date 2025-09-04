using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RefundPolicy : BaseAuditableEntity
{
    [Required]
    public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;

    // % refund if canceled before this many hours prior to check-in
    public int HoursBeforeCheckIn { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal RefundPercent { get; set; }   // e.g., 100%, 50%, 0%

    // Whether deposit is refundable under this rule
    public bool RefundDeposit { get; set; } = false;

    // Policy effective period (can change over time)
    public DateTime EffectiveFrom { get; set; }
    public DateTime EffectiveTo { get; set; }
}
