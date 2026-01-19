using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class FeeCategory : BaseAuditableEntity
{
    [Required, MaxLength(50)]
    public string Code { get; set; } = default!;   // A, B, C

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;   // Category A
}

