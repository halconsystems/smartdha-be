using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class ServiceCategory : BaseAuditableEntity
{
    // Example: NOC / Lease / Mutation / Site Plan
    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(30)]
    public string Code { get; set; } = default!;
}

