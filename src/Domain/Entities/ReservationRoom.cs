using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ReservationRoom : BaseAuditableEntity
{
    [Required] public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = default!;

    [Required] public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;

    [Required] public DateTime FromDate { get; set; }
    [Required] public DateOnly FromDateOnly { get; set; }
    [Required] public TimeOnly FromTimeOnly { get; set; }
    [Required] public DateTime ToDate { get; set; }
    [Required] public DateOnly ToDateOnly { get; set; }
    [Required] public TimeOnly ToTimeOnly { get; set; }

    // Snapshot pricing (freeze price at reservation time)
    [Column(TypeName = "decimal(18,2)")] public decimal PricePerNight { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Subtotal { get; set; }
}
