using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;

public class LaundryPackaging :BaseAuditableEntity
{
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public string DisplayName { get; set; } = default!;
    public string? Code { get; set; }

    
}
