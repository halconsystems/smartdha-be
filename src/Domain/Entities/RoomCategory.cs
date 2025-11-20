using System;
using System.ComponentModel.DataAnnotations;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class RoomCategory : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public ClubType ClubType { get; set; } = ClubType.GuestRoom;

    [Required]
    public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}


