using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DHAFacilitationAPIs.Domain.Common;

namespace DHAFacilitationAPIs.Domain.Entities;

public class ResidenceType : BaseAuditableEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
