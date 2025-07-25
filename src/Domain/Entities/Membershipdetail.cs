using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class Membershipdetail : BaseAuditableEntity
{
    public string UserId { get; set; } = default!;
    public string MembershipId { get; set; } = default!;
    public string MembershipName { get; set;} = default!;

}
