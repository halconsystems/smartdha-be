using System;
using System.Collections.Generic;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions.Dtos;

public class ModulePermissionDto
{
    public Guid ModuleId { get; set; }
    public List<SubModulePermissionDto> SubModules { get; set; } = new();
}
