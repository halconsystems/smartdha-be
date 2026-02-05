using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.CBMS;
public class ServiceDefinition : BaseAuditableEntity
{
    public Guid ClubServiceCategoryId { get; set; }   // Sports / Events
    public ClubServiceCategory ClubServiceCategory { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public bool IsQuantityBased { get; set; }
}

