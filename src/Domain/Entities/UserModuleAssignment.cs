using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserModuleAssignment : BaseAuditableEntity
{
    [Required]
    public string UserId { get; set; } = default!;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = default!;

    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = default!;
}


