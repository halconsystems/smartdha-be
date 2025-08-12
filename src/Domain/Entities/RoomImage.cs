using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RoomImage : BaseAuditableEntity
{
    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public string ImageURL { get; set; } = default!;

    [Required]
    public string ImageExtension { get; set; } = default!;

    public string? ImageName { get; set; }
    public string? Description { get; set; }

    [Required]
    public ImageCategory Category { get; set; } = ImageCategory.Main; // default to Main
}

