using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetAccessTree;
public class PermissionTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool Checked { get; set; }
}

public class SubModuleTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool Checked { get; set; }
    public List<PermissionTreeDto> Permissions { get; set; } = new();
}

public class ModuleTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool Checked { get; set; }
    public List<SubModuleTreeDto> SubModules { get; set; } = new();
}

