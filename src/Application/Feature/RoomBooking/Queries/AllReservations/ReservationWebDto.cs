using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.AllReservations;
public class ReservationWebDto
{
    public Guid ReservationId { get; set; }
    public string OneBillId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;

    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }

    // Amounts
    public decimal RoomsAmount { get; set; }
    public decimal Taxes { get; set; }
    public decimal Discounts { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }

    // User
    public UserInfoDto User { get; set; } = default!;

    // Club
    public ClubInfoDto Club { get; set; } = default!;

    // Guest (optional)
    public CreateGuestDto? Guest { get; set; }

    // Rooms
    public List<ReservationRoomDetailDto> Rooms { get; set; } = new();

    // Payments
    public List<PaymentIntentDto> PaymentIntents { get; set; } = new();
}

public class UserInfoDto
{
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

