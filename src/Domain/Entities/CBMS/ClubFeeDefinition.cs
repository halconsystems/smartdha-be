using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;

public class ClubFeeDefinition : BaseAuditableEntity
{
    public Guid ProcessId { get; set; }
    public ClubServiceProcess Process { get; set; } = default!;

    public FeeType FeeType { get; set; } = FeeType.Fixed;

    // For Fixed fee
    public decimal? FixedAmount { get; set; }

    // For AreaBased
    public AreaUnit? AreaUnit { get; set; }

    // If fee can be overridden for special cases
    public bool AllowOverride { get; set; } = true;

    // Validity (optional)
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    [MaxLength(200)]
    public string? Notes { get; set; }
}
