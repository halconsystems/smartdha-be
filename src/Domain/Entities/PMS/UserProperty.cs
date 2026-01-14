using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class UserProperty : BaseAuditableEntity
{
    // DHA property reference
    [Required, MaxLength(50)]
    public string PropertyNo { get; set; } = default!;

    [MaxLength(100)]
    public string? Sector { get; set; }

    [MaxLength(100)]
    public string? Phase { get; set; }

    // Owner CNIC (optional if you have user module)
    [MaxLength(20)]
    public string? OwnerCnic { get; set; }
}

