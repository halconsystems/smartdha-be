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

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    public UserType AppType { get; set; } // Web or Mobile

    public ICollection<SubModule> SubModules { get; set; } = new List<SubModule>();
    public ICollection<UserModuleAssignment> UserAssignments { get; set; } = new List<UserModuleAssignment>();
}

