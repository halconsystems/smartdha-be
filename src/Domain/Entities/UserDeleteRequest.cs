using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserDeleteRequest : BaseAuditableEntity
{
    public Guid UserId { get; set; }

    public string? Reason { get; set; }
}
