using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class Room : BaseAuditableEntity
{
    [Required]
    public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;

    [Required]
    public Guid RoomCategoryId { get; set; }
    public RoomCategory RoomCategory { get; set; } = default!;

    [Required]
    public Guid ResidenceTypeId { get; set; }
    public ResidenceType ResidenceType { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string No { get; set; } = default!;

    [Required]
    public int MaxOccupancy { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }
    public string? Description { get; set; }

    // Quick global toggle (optional, see #2)
    public bool IsGloballyAvailable { get; set; } = true;

    public ICollection<Services> RoomServices { get; set; } = new List<Services>();
    public ICollection<RoomAvailability> Availabilities { get; set; } = new List<RoomAvailability>();
}
