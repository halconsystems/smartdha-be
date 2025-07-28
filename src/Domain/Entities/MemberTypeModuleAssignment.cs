using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class MemberTypeModuleAssignment : BaseAuditableEntity
{
    [Required]
    public UserType UserType { get; set; }  // Now supports Member, NonMember, Employee

    [Required]
    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = default!;
}

