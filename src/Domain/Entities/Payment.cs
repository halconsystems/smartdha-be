using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Payment : BaseAuditableEntity
{
    [Required] public Guid PaymentIntentId { get; set; }
    public PaymentIntent PaymentIntent { get; set; } = default!;

    [Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Authorized;
    public DateTime? PaidAt { get; set; }

    public PaymentMethod Method { get; set; }
    public PaymentProvider Provider { get; set; } = PaymentProvider.None;

    [MaxLength(100)] public string? ProviderTransactionId { get; set; }
    public string? RawResponse { get; set; } // gateway payload for audits

    // Optional: Once Booking exists, also record allocation against it
    public Guid? BookingId { get; set; }
    public RoomBooking? Booking { get; set; }
}
