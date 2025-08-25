using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class AppRolePermission : BaseAuditableEntity
{
    public Guid RoleId { get; set; }
    public AppRole Role { get; set; } = default!;

    public Guid SubModuleId { get; set; }
    public SubModule SubModule { get; set; } = default!;

    // Actions allowed (Approve, Reject, Write, etc.)
    public string AllowedActions { get; set; } = string.Empty; // CSV/JSON
}

