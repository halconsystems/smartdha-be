using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class FacilityUnitService : BaseAuditableEntity
{
    public Guid FacilityUnitId { get; set; }
    public FacilityUnit FacilityUnit { get; set; } = default!;

    public Guid ServiceDefinitionId { get; set; }
    public ServiceDefinition ServiceDefinition { get; set; } = default!;

    public decimal Price { get; set; }
    public bool IsComplimentary { get; set; }
    public bool IsEnabled { get; set; } = true;
}

