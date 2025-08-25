using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.SubModules.Queries.SubModuleList;
public class PermissionDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class AllSubModulesDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ModuleId { get; set; }
    public bool RequiresPermission { get; set; }

    public List<PermissionDto> Permissions { get; set; } = new();
}


