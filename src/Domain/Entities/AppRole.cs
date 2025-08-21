using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Domain.Entities;
public class AppRole : BaseAuditableEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;
    public bool IsSystemRole { get; set; } = false;

    // Scope of this role
    public Guid? ClubId { get; set; } // null = global role

    // Role ↔ Modules
    public ICollection<AppRoleModule> RoleModules { get; set; } = new List<AppRoleModule>();

    // Role ↔ SubModule Permissions
    public ICollection<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();

    // Role ↔ Users
    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}


