using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserById;
public class UserDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string MEMPK { get; set; } = default!;
    public AppType AppType { get; set; }
    public UserType UserType { get; set; }
    public string? RegisteredMobileNo { get; set; }
    public string? RegisteredEmail { get; set; }
    public string? RegistrationNo { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOtpRequired { get; set; }

    public List<ModuleAssignmentDto> ModuleAssignments { get; set; } = new();
}

public class ModuleAssignmentDto
{
    public Guid? ModuleId { get; set; }
    public string ModuleName { get; set; } = default!;
    public string? Description { get; set; }
    public string? Title { get; set; }
    public string? Remarks { get; set; }

    public List<SubModuleAssignmentDto> SubModules { get; set; } = new();
}

public class SubModuleAssignmentDto
{
    public Guid SubModuleId { get; set; }
    public string SubModuleName { get; set; } = default!;
    public bool RequiresPermission { get; set; }

    public List<UserPermissionDto> Permissions { get; set; } = new();
}

public class UserPermissionDto
{
    public Guid Id { get; set; }
    public string AllowedActions { get; set; } = string.Empty; // CSV/JSON
}

