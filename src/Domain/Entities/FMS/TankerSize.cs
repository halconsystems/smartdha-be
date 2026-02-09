using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.FMS;

public class TankerSize :BaseAuditableEntity
{
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public string DisplayName { get; set; } = default!;
    public string? Code { get; set; }
    [Required]
    public string Price { get; set; } = default!;
    public Guid? FemServiceId { get; set; }
    public FemService? FemService { get; set; }
   


}
