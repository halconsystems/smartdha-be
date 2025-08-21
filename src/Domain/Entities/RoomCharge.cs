using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHAFacilitationAPIs.Domain.Entities;
public class RoomCharge : BaseAuditableEntity
{
    [Required]
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;

    [Required]
    public RoomBookingType BookingType { get; set; } = RoomBookingType.Self;

    [Required]
    public int NoOfOccupancy { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Charges { get; set; }

}
