using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ComplaintCategory : BaseAuditableEntity
{
    [Required]
    [MaxLength(200)]
    public string Code { get; set; } = default!;

    [Required]
    [MaxLength(300)]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }
    public string? ColorCode { get; set; }    // e.g. "#1E88E5" or "rgb(30,136,229)"
}
