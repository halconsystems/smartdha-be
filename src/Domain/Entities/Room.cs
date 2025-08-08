using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class Room : BaseAuditableEntity
{
    [Required]
    public Guid ClubId { get; set; }

    [Required]
    public Guid RoomCategoryId { get; set; }

    [Required]
    public Guid ResidenceTypeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string No { get; set; } = default!;

    [MaxLength(100)]
    public string? Name { get; set; }

    [Required]
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }
}
