using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserSubModuleAssignment : BaseAuditableEntity
{
    [Required]
    public string UserId { get; set; } = default!;
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = default!;

    public Guid SubModuleId { get; set; }
    public SubModule SubModule { get; set; } = default!;
}
