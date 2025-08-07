using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RoomCharges : BaseAuditableEntity
{
    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public string BookingType { get; set; } = default!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Charges { get; set; }

}
