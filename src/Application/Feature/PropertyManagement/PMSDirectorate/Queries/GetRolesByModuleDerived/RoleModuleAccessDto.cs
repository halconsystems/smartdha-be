using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSDirectorate.Queries.GetRolesByModuleDerived;
public class RoleModuleAccessDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = default!;
    public bool IsAssigned { get; set; }
}
