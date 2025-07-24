using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class SubModule : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;
    public Guid? ModuleId { get; set; }
    public Module Module { get; set; } = default!;
}

