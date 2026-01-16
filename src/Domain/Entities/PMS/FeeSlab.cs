using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class FeeSlab : BaseAuditableEntity
{
    public Guid FeeDefinitionId { get; set; }
    public FeeDefinition FeeDefinition { get; set; } = default!;

    public decimal FromArea { get; set; }     // inclusive
    public decimal ToArea { get; set; }       // inclusive (use very big number for open-ended)
    public decimal Amount { get; set; }
}

