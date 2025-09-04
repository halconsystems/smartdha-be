using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.BowserCapacities.Queries;
public class BowserCapacityDto
{
    public Guid Id { get; set; }
    public decimal Capacity { get; set; }
    public string Unit { get; set; } = default!;
}
