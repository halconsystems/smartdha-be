using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ExtraServiceCharges : BaseAuditableEntity
{
    [Required]
    public Guid ServiceId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Charges { get; set; }

}
