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
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class AllModulesDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<AllSubModulesDto> AllSubModules { get; set; } = new();
  
}


