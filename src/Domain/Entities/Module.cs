using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class Module : BaseAuditableEntity
{

    [Required, MaxLength(100)]
    public string Value { get; set; } = default!;         // e.g. "ClubManagement"
    [Required, MaxLength(100)]
    public string DisplayName { get; set; } = default!;   // e.g. "Club Management"

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;
    
    [MaxLength(100)]
    public string Description { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Remarks { get; set; } = default!;
    public AppType AppType { get; set; } = default!; // Web or Mobile
    public string? URL { get; set; } 

    public ICollection<SubModule> SubModules { get; set; } = new List<SubModule>();
    public ICollection<UserModuleAssignment> UserAssignments { get; set; } = new List<UserModuleAssignment>();
}

