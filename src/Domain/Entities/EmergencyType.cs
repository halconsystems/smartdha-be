using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class EmergencyType : BaseAuditableEntity
{
    // Keep "Code" to preserve your old enum values (1,2,3,4,99)
    public int Code { get; set; }              // 1,2,3,4,99
    [Required, MaxLength(100)] public string Name { get; set; } = default!;
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(30)] public string? HelplineNumber { get; set; }
}
