using System;
using System.ComponentModel.DataAnnotations;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class Services : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string ServiceName { get; set; } = default!;

    public string? Description { get; set; }
}
