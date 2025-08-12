using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Reservation : BaseAuditableEntity
{
    [Required] public Guid UserId { get; set; } = default!; // Member making the hold
    [Required] public Guid ClubId { get; set; }
    public Club Club { get; set; } = default!;

    public ReservationStatus Status { get; set; } = ReservationStatus.AwaitingPayment;

    [Required] public DateTime ExpiresAt { get; set; }      // hard TTL for hold

    // Pricing snapshot at reservation time
    [Column(TypeName = "decimal(18,2)")] public decimal RoomsAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Taxes { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Discounts { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal TotalAmount { get; set; }

    // Deposit policy for this reservation
    [Column(TypeName = "decimal(5,2)")] public decimal DepositPercentRequired { get; set; } // e.g., 30.00
    [Column(TypeName = "decimal(18,2)")] public decimal DepositAmountRequired { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal AmountPaidSoFar { get; set; } = 0m;

    // Optional guest (if booking for a guest)
    public Guid? GuestId { get; set; }
    public BookingGuest? Guest { get; set; }

    public ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();
    public ICollection<PaymentIntent> PaymentIntents { get; set; } = new List<PaymentIntent>();
}
