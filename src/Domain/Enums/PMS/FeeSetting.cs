using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Enums.PMS;
public class FeeSetting : BaseAuditableEntity
{
    public string Code { get; set; } = default!;     // NADRA_FEE, SERVICE_FEE
    public string DisplayName { get; set; } = default!;
    public decimal Amount { get; set; }
}
