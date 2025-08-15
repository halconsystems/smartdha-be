using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class ServiceMapping : BaseAuditableEntity
{
    [Required]
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;

    [Required]
    public Guid ServiceId { get; set; }

    [ForeignKey(nameof(ServiceId))]
    public Services Services { get; set; } = default!;
}
