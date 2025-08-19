using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ConfirmBooking.Commands;
public class BookingConfirmationDto
{
    public Guid ReservationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public List<BookingDetailDto> Bookings { get; set; } = new();
}

public class BookingDetailDto
{
    public Guid BookingId { get; set; }
    public Guid RoomId { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

