using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RoleAssignment : BaseAuditableEntity
{
    public string ParentRole { get; set; } = default!;   // e.g. "Super Administrator"
    public string ChildRole { get; set; } = default!;    // e.g. "Administrator"
}
