using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class FeeOption : BaseAuditableEntity
{
    public Guid FeeDefinitionId { get; set; }
    public FeeDefinition FeeDefinition { get; set; } = default!;

    // Optional: Category A / B
    public Guid? FeeCategoryId { get; set; }
    public FeeCategory? FeeCategory { get; set; }

    [Required, MaxLength(50)]
    public string Code { get; set; } = default!;   // ORDINARY / URGENT

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;   // Ordinary / Urgent

    public int ProcessingDays { get; set; }

    public decimal Amount { get; set; }

    public int SortOrder { get; set; }
}

