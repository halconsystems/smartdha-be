using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class CaseFee : BaseAuditableEntity
{
    public Guid CaseId { get; set; }
    public PropertyCase Case { get; set; } = default!;

    public Guid? FeeDefinitionId { get; set; }
    public FeeDefinition? FeeDefinition { get; set; }

    // What was used as input
    public decimal? PropertyArea { get; set; }
    public AreaUnit? AreaUnit { get; set; }

    // Calculation result
    public decimal Amount { get; set; }

    // Optional: if slab used
    public Guid? FeeSlabId { get; set; }
    public FeeSlab? FeeSlab { get; set; }

    // If Finance overrides
    public bool IsOverridden { get; set; } = false;

    [MaxLength(500)]
    public string? Remarks { get; set; }
}

