using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations.Dtos;
public class ReservationCardDto
{
    public Guid ReservationId { get; set; }
    public string OneBillId { get; set; } = default!;
    public string ClubName { get; set; } = default!;
    public string ClubLocation { get; set; } = default!;

    public DateTime FromDate { get; set; }   // earliest check-in
    public DateTime ToDate { get; set; }     // latest checkout
    public int Nights { get; set; }
    public int RoomsCount { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public string PaymentStatus { get; set; } = default!; // "Paid", "Pending", "Partially Paid"

    public string Status { get; set; } = default!; // Reservation status (AwaitingPayment, Converted, etc.)
    public DateTime CreatedDateTime { get; set; } = default!;
}

