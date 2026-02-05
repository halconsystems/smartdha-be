using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ClubMembership : BaseAuditableEntity
{
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string? MembershipNo { get; set; } = default!;
    public string? Rank { get; set; } = default!;
    public string? Name { get; set; } = default!;
    public string? CNIC { get; set; } = default!;
    public string? MobileNumber { get; set; } = default!;
    public string? OneInKid { get; set; } = default!;
    public string? BillStatus { get; set; } = default!;
    public string? Clube { get; set; } = default!;
}

