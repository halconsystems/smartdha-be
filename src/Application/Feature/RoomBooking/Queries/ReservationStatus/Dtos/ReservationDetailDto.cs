using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation.Dtos;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;
public class ReservationDetailDto
{
    public Guid ReservationId { get; set; }
    public string OneBillId { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }

    // Pricing
    public decimal RoomsAmount { get; set; }
    public decimal Taxes { get; set; }
    public decimal Discounts { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaidSoFar { get; set; }
    public decimal DepositPercentRequired { get; set; }
    public decimal DepositAmountRequired { get; set; }

    // People
    public int Adult { get; set; }
    public int Child { get; set; }
    public CreateGuestDto? Guest { get; set; }

    // Related
    public ClubInfoDto Club { get; set; } = default!;
    public List<ReservationRoomDetailDto> Rooms { get; set; } = new();
    public List<PaymentIntentDto> Payments { get; set; } = new();
    public Reservationstage Reservationstage { get; set; } = default!;
    public Paymentstage Paymentstage { get; set; } = default!;
    public Bookingstage Bookingstage { get; set; } = default!;

}
public class Reservationstage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public int StatusCode { get; set; } = 0;
}
public class Paymentstage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public int StatusCode { get; set; } = 0;
}
public class Bookingstage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public int StatusCode { get; set; } = 0;
}

public class ReservationRoomDetailDto
{
    public Guid RoomId { get; set; }
    public string RoomNo { get; set; } = default!;
    public string? RoomName { get; set; }
    public string CategoryName { get; set; } = default!;
    public string ResidenceType { get; set; } = default!;
    public List<string> Services { get; set; } = new();

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalNights { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal Subtotal { get; set; }
}

public class PaymentIntentDto
{
    public Guid PaymentIntentId { get; set; }
    public decimal AmountToCollect { get; set; }
    public bool IsDeposit { get; set; }
    public string Status { get; set; } = default!;
    public string Method { get; set; } = default!;
    public string Provider { get; set; } = default!;
    public List<PaymentDto> Payments { get; set; } = new();
}

public class PaymentDto
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? PaidAt { get; set; }
    public string Method { get; set; } = default!;
    public string Provider { get; set; } = default!;
}

