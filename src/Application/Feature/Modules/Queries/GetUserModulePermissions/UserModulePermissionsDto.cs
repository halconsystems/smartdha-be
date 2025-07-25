using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Queries.GetUserModulePermissions;
public class UserModulePermissionsDto
{
    public List<ModuleDto> Modules { get; set; } = new();
}

public class ModuleDto
{
    public Guid ModuleId { get; set; }
    public string ModuleName { get; set; } = default!;
    public List<SubModuleDto> SubModules { get; set; } = new();
}

public class SubModuleDto
{
    public Guid SubModuleId { get; set; }
    public string SubModuleName { get; set; } = default!;
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}
