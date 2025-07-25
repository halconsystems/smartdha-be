using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetAssignableModules;
public class ModuleWithSubDto
{
    public Guid ModuleId { get; set; }
    public string ModuleName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public List<SubModuleDto> SubModules { get; set; } = new();
}

public class SubModuleDto
{
    public Guid SubModuleId { get; set; }
    public string Description { get; set; } = default!;
    public string SubModuleName { get; set; } = default!;
}

