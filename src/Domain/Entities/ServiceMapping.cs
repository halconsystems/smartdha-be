using System;
using System.ComponentModel.DataAnnotations;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class ServiceMapping : BaseAuditableEntity
{
    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public Guid ServiceId { get; set; }
}
