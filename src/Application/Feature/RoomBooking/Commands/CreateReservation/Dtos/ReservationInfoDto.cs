using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation.Dtos;

public class ReservationInfoDto
{
    public string OneBillId { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public ClubInfoDto Club { get; set; } = default!;
    public List<ReservationRoomInfoDto> Rooms { get; set; } = new();
}
public class ClubInfoDto
{
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
}

public class ReservationRoomInfoDto
{
    public string RoomNo { get; set; } = default!;
    public string? RoomName { get; set; }
    public string? Description { get; set; }

    // Relations
    public string CategoryName { get; set; } = default!;
    public string ResidenceType { get; set; } = default!;
    public List<string> Services { get; set; } = new();

    // Booking info
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalNights { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal Subtotal { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
}
