using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class UserClubAssignment : BaseAuditableEntity
{
    [Required]
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    [Required]
    public Guid ClubId { get; set; }

}

