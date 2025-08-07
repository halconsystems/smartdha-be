using System;
using System.ComponentModel.DataAnnotations;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class RoomCategory : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = default!;
    public string? Description { get; set; }
}
