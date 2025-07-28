using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class MembershipPurpose : BaseAuditableEntity
{
    [Required]
    public string Title { get; set; } = default!;
    public string? Description { get; set; } = default!;
}
