using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RoomImages : BaseAuditableEntity
{
    [Required]
    public Guid RoomId { get; set; }

    public string ImageURL { get; set; } = default!;

}
