using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class AppPermission : BaseAuditableEntity
{
    [Required, MaxLength(100)]
    public string Value { get; set; } = default!;         // e.g. "Approve"
    [Required, MaxLength(100)]
    public string DisplayName { get; set; } = default!;   // e.g. "Approve Request"

    public Guid SubModuleId { get; set; }
    public SubModule SubModule { get; set; } = default!;
}
