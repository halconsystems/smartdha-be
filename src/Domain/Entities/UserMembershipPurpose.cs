using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserMembershipPurpose : BaseAuditableEntity
{
    [Required]
    public string UserId { get; set; } = default!;

    [Required]
    public Guid PurposeId { get; set; } = default!;

    // Navigation properties (optional)
    public ApplicationUser? User { get; set; }
    public MembershipPurpose? Purpose { get; set; }
}

