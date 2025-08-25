using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserAccessById;
public class UserAccessViewDto
{
    public string UserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string Role { get; set; } = default!;

    public List<ModuleAccessDto> Modules { get; set; } = new();
}

public class ModuleAccessDto
{
    public Guid ModuleId { get; set; }
    public string ModuleName { get; set; } = default!;
    public bool IsAlreadyAssigned { get; set; }
    public List<SubModuleAccessDto> SubModules { get; set; } = new();
}

public class SubModuleAccessDto
{
    public Guid SubModuleId { get; set; }
    public string SubModuleName { get; set; } = default!;
    public bool IsAlreadyAssigned { get; set; }
    public bool RequiresPermission { get; set; }
    public List<PermissionAccessDto> Permissions { get; set; } = new();
}

public class PermissionAccessDto
{
    public Guid PermissionId { get; set; }
    public string DisplayName { get; set; } = default!;
    public string Value { get; set; } = default!;
    public bool IsAlreadyAssigned { get; set; }
}

