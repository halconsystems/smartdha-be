using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RolePermission : BaseAuditableEntity
{
    public string RoleName { get; set; } = default!;
    public Guid? SubModuleId { get; set; }
    public SubModule? SubModule { get; set; }

    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}

