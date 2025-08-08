using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RoomRatings : BaseAuditableEntity
{
    [Required]
    public Guid RoomId { get; set; }

    [Column(TypeName = "decimal(3,1)")]
    [Range(0, 5)]
    public decimal RoomRating { get; set; }
}
