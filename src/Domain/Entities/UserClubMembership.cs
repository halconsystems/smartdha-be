using System;
using System.ComponentModel.DataAnnotations;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class UserClubMembership : BaseAuditableEntity
{
    [Required]
    public string UserId { get; set; } = default!;

    [Required]
    public Guid ClubId { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
