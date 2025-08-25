using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class AppUserRole : BaseAuditableEntity
{
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public Guid RoleId { get; set; }
    public AppRole Role { get; set; } = default!;
}
