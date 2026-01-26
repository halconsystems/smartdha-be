using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;

public class ClubFeeSlab : BaseAuditableEntity
{
    public Guid FeeDefinitionId { get; set; }
    public ClubFeeDefinition FeeDefinition { get; set; } = default!;

    public decimal FromArea { get; set; }     // inclusive
    public decimal ToArea { get; set; }       // inclusive (use very big number for open-ended)
    public decimal Amount { get; set; }
}
