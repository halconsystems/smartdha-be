using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ClubCategory : BaseAuditableEntity
{
    // Example: NOC / Lease / Mutation / Site Plan
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;
    [Required, MaxLength(200)]
    public string DisplayName { get; set; } = default!;

    [Required, MaxLength(30)]
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
}
