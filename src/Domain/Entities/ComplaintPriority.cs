using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ComplaintPriority : BaseAuditableEntity
{
    [Required]
    [MaxLength(200)]
    public string Code { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required]
    [MaxLength(300)]
    public int Weight { get; set; }  // 1=Low, 2=Medium, 3=High, etc.
}
