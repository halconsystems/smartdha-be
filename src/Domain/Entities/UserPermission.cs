using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserPermission : BaseAuditableEntity
{
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public Guid SubModuleId { get; set; }
    public SubModule SubModule { get; set; } = default!;

    public string AllowedActions { get; set; } = string.Empty; // CSV/JSON like "Create,Update,Delete"
}

