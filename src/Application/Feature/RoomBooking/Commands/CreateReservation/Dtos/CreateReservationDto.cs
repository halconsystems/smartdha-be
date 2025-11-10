// Imports for DTO file
using System;
using System.Collections.Generic;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;

public class CreateReservationDto
{
    public Guid ClubId { get; set; }
    public RoomBookingType BookingType { get; set; } = RoomBookingType.Self;
    public List<CreateReservationRoomDto> Rooms { get; set; } = new();
    public CreateGuestDto? Guest { get; set; }
}

public class CreateReservationRoomDto
{
    public Guid RoomId { get; set; }
    public DateTime FromDate { get; set; }   // inclusive check-in
    public DateTime ToDate { get; set; }   // exclusive or next-day checkout (your current logic uses nightly count = To-From) => Room Booking 
                                           // same day checkout => Ground Booking
    public int Adults { get; set; } = 0;
    public int Children { get; set; } = 0;
}

public class CreateGuestDto
{
    public string FullName { get; set; } = default!;
    public string? CNICOrPassport { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}
