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
    public string Value { get; set; } = default!;         // e.g. "ClubManagement"
    public Boolean AlreadyAccess { get; set; } = false;
    public string DisplayName { get; set; } = default!;   // e.g. "Club Management"
}

public class SubModuleTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;         // e.g. "ClubManagement"
    public string DisplayName { get; set; } = default!;   // e.g. "Club Management"
    public Boolean AlreadyAccess { get; set; } = false;
    public List<PermissionTreeDto> Permissions { get; set; } = new();
}

public class ModuleTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;         // e.g. "ClubManagement"
    public string DisplayName { get; set; } = default!;   // e.g. "Club Management"
    public Boolean AlreadyAccess { get; set; } = false;
    public List<SubModuleTreeDto> SubModules { get; set; } = new();
}

