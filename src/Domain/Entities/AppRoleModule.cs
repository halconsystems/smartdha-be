using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class AppRoleModule : BaseAuditableEntity
{
    public Guid RoleId { get; set; }
    public AppRole Role { get; set; } = default!;

    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = default!;
}

