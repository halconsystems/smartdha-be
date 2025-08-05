using System;

namespace DHAFacilitationAPIs.Application.Feature.Modules.Commands.UpdateUserModulePermissions.Dtos;

public class SubModulePermissionDto
{
    public Guid SubModuleId { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}
