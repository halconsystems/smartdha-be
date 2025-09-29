using System;
using System.ComponentModel.DataAnnotations;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class Services : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public ServiceType ServiceType { get; set; } = ServiceType.GuestRoom;

}
