using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation.Dtos;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.ConfirmPaidRoomBooking;
public record ConfirmPaidRoomBookingCommand : IRequest<ReservationInfoDto>
{
    // Payment info (from IPG)
    public string? ProviderIntentId { get; init; } = default!;
    public string? ProviderTransactionId { get; init; } = default!;
    public PaymentProvider Provider { get; init; } = PaymentProvider.EasyPaisa;
    public PaymentMethod Method { get; init; } = PaymentMethod.Wallet;
    public decimal PaidAmount { get; init; }
    public string? RawResponse { get; init; } = "Success";

    // Booking info (original request snapshot)
    public Guid ClubId { get; init; }
    public RoomBookingType BookingType { get; init; }
    public List<CreateReservationRoomDto> Rooms { get; init; } = new();
    public CreateGuestDto? Guest { get; init; }
}
public class ConfirmPaidRoomBookingHandler
    : IRequestHandler<ConfirmPaidRoomBookingCommand, ReservationInfoDto>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ConfirmPaidRoomBookingHandler(IOLMRSApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ReservationInfoDto> Handle(
    ConfirmPaidRoomBookingCommand cmd,
    CancellationToken ct)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            Guid currentUserId = _currentUserService.UserId;

            var roomIds = cmd.Rooms.Select(x => x.RoomId).Distinct().ToList();

            var roomsMap = await _context.Rooms
                .AsNoTracking()
                .Where(r => roomIds.Contains(r.Id))
                .Select(r => new
                {
                    r.Id,
                    r.ClubId,
                    r.NormalOccupancy,
                    r.MaxExtraOccupancy
                })
                .ToDictionaryAsync(r => r.Id, ct);

            if (roomsMap.Values.Any(r => r.ClubId != cmd.ClubId))
                throw new InvalidOperationException("Room does not belong to club.");

            var chargesDict = await _context.RoomCharges
                .Where(c =>
                    roomIds.Contains(c.RoomId) &&
                    c.BookingType == cmd.BookingType &&
                    c.IsActive==true &&
                    c.IsDeleted==false )
                .ToDictionaryAsync(
                    c => (c.RoomId, c.ExtraOccupancy),
                    c => c.Charges,
                    ct);

            decimal roomsAmount = 0m;
            var reservationRooms = new List<ReservationRoom>();

            foreach (var r in cmd.Rooms)
            {
                var nights = (r.ToDate.Date - r.FromDate.Date).Days;
                if (nights <= 0)
                    throw new InvalidOperationException("Invalid date range");

                if (!roomsMap.TryGetValue(r.RoomId, out var room))
                    throw new InvalidOperationException($"Room not found: {r.RoomId}");

                var occupants = r.Adults + r.Children;
                var extra = Math.Max(0, occupants - room.NormalOccupancy);

                if (extra > room.MaxExtraOccupancy)
                    throw new InvalidOperationException("Extra occupancy exceeded");

                if (!chargesDict.TryGetValue((r.RoomId, extra), out var nightly))
                    throw new InvalidOperationException("Room charges missing");

                var subtotal = nightly * nights;
                roomsAmount += subtotal;

                reservationRooms.Add(new ReservationRoom
                {
                    RoomId = r.RoomId,
                    FromDate = r.FromDate,
                    ToDate = r.ToDate,
                    FromDateOnly = DateOnly.FromDateTime(r.FromDate),
                    ToDateOnly = DateOnly.FromDateTime(r.ToDate),
                    FromTimeOnly = TimeOnly.FromDateTime(r.FromDate),
                    ToTimeOnly = TimeOnly.FromDateTime(r.ToDate),
                    PricePerNight = nightly,
                    Subtotal = subtotal
                });
            }

            if (roomsAmount != cmd.PaidAmount)
                throw new InvalidOperationException("Payment tampering detected");

            var reservation = new Reservation
            {
                UserId = currentUserId,
                ClubId = cmd.ClubId,
                Status = ReservationStatus.Converted,
                RoomsAmount = roomsAmount,
                TotalAmount = roomsAmount,
                AmountPaidSoFar = cmd.PaidAmount
            };

            foreach (var rr in reservationRooms)
                reservation.ReservationRooms.Add(rr);

            _context.Reservations.Add(reservation);

            foreach (var rr in reservationRooms)
            {
                _context.RoomBookings.Add(new Domain.Entities.RoomBooking
                {
                    Reservation = reservation,
                    UserId = currentUserId,
                    ClubId = cmd.ClubId,
                    RoomId = rr.RoomId,
                    Status = BookingStatus.Confirmed,
                    CheckInDate = rr.FromDate,
                    CheckInDateOnly = rr.FromDateOnly,
                    CheckInTimeOnly = rr.FromTimeOnly,
                    CheckOutDate = rr.ToDate,
                    CheckOutDateOnly = rr.ToDateOnly,
                    CheckOutTimeOnly = rr.ToTimeOnly,
                    TotalAmount = rr.Subtotal
                });
            }

            var intent = new PaymentIntent
            {
                Reservation = reservation,
                AmountToCollect = cmd.PaidAmount,
                Status = PaymentIntentStatus.Succeeded,
                Provider = cmd.Provider,
                Method = cmd.Method,
                ProviderIntentId = cmd.ProviderIntentId
            };

            _context.PaymentIntents.Add(intent);

            _context.Payments.Add(new Payment
            {
                PaymentIntent = intent,
                Amount = cmd.PaidAmount,
                Status = PaymentStatus.Paid,
                Provider = cmd.Provider,
                Method = cmd.Method,
                ProviderTransactionId = cmd.ProviderTransactionId,
                PaidAt = DateTime.Now,
                RawResponse = cmd.RawResponse
            });

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            // Build response
            var persisted = await _context.Reservations
    .AsNoTracking()
    .Include(r => r.RoomBookings)
        .ThenInclude(rb => rb.Room)
            .ThenInclude(room => room.RoomCategory)
    .Include(r => r.RoomBookings)
        .ThenInclude(rb => rb.Room)
            .ThenInclude(room => room.ResidenceType)
    .FirstOrDefaultAsync(r => r.Id == reservation.Id, ct)
    ?? throw new InvalidOperationException($"Reservation {reservation.Id} not found after save.");


            var club = await _context.Clubs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == cmd.ClubId, ct)
            ?? throw new InvalidOperationException($"Club with Id {cmd.ClubId} not found.");

            var result = new ReservationInfoDto
            {
                Status = persisted.Status.ToString(),

                Club = new ClubInfoDto
                {
                    Name = club.Name,
                    Location = club.Location,
                    ContactNumber = club.ContactNumber
                },

                Rooms = persisted.RoomBookings.Select(rb => new ReservationRoomInfoDto
                {
                    RoomNo = rb.Room?.No ?? string.Empty,
                    RoomName = rb.Room?.Name,
                    Description = rb.Room?.Description,
                    CategoryName = rb.Room?.RoomCategory?.Name ?? string.Empty,
                    ResidenceType = rb.Room?.ResidenceType?.Name ?? string.Empty,

                    Services = _context.ServiceMappings
                        .Where(sm => sm.RoomId == rb.RoomId && (sm.IsDeleted == false || sm.IsDeleted == null))
                        .Select(sm => sm.Services.Name)
                        .ToList(),

                    FromDate = rb.CheckInDate!.Value,
                    ToDate = rb.CheckOutDate!.Value,
                    TotalNights = (rb.CheckOutDate!.Value.Date - rb.CheckInDate!.Value.Date).Days,

                    PricePerNight = rb.TotalAmount /
                                    Math.Max(1, (rb.CheckOutDate!.Value.Date - rb.CheckInDate!.Value.Date).Days),

                    Subtotal = rb.TotalAmount,

                    // ✅ THIS IS WHAT YOU ASKED FOR
                    BookingStatus = rb.Status.ToString() // Confirmed
                }).ToList()
            };

            return result;



        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw; // 🚨 THIS IS MANDATORY
        }
    }


    //public async Task<Guid> Handle(
    //    ConfirmPaidRoomBookingCommand cmd,
    //    CancellationToken ct)
    //{
    //    using var tx = await _context.Database.BeginTransactionAsync(ct);
    //    try
    //    {

    //        Guid currentUserId = _currentUserService.UserId;

    //        // 🔒 Idempotency guard
    //        //var alreadyProcessed = await _context.Payments
    //        //    .AnyAsync(p => p.ProviderTransactionId == cmd.ProviderTransactionId, ct);

    //        //if (alreadyProcessed)
    //        //    throw new InvalidOperationException("Payment already processed");

    //        var roomIds = cmd.Rooms.Select(x => x.RoomId).Distinct().ToList();

    //        var roomsMap = await _context.Rooms
    //            .AsNoTracking()
    //            .Where(r => roomIds.Contains(r.Id))
    //            .Select(r => new
    //            {
    //                r.Id,
    //                r.ClubId,
    //                r.NormalOccupancy,
    //                r.MaxExtraOccupancy
    //            })
    //            .ToDictionaryAsync(r => r.Id, ct);

    //        if (roomsMap.Values.Any(r => r.ClubId != cmd.ClubId))
    //            throw new InvalidOperationException("Room does not belong to club.");

    //        var chargesDict = await _context.RoomCharges
    //            .Where(c => roomIds.Contains(c.RoomId)
    //                && c.BookingType == cmd.BookingType && c.IsDeleted == false && c.IsActive == true)
    //            .ToDictionaryAsync(
    //                c => (c.RoomId, c.ExtraOccupancy),
    //                c => c.Charges,
    //                ct);

    //        // 1️⃣ PRICE RE-CALCULATION (MANDATORY)
    //        decimal roomsAmount = 0;
    //        var reservationRooms = new List<ReservationRoom>();


    //        foreach (var r in cmd.Rooms)
    //        {
    //            var nights = (r.ToDate.Date - r.FromDate.Date).Days;
    //            if (nights <= 0)
    //                throw new InvalidOperationException("ToDate must be after FromDate.");

    //            if (!roomsMap.TryGetValue(r.RoomId, out var roomInfo))
    //                throw new InvalidOperationException($"Room not found: {r.RoomId}");

    //            // 👇 IMPORTANT PART YOU WERE MISSING
    //            var occupants = r.Adults + r.Children;
    //            var extra = Math.Max(0, occupants - roomInfo.NormalOccupancy);

    //            if (extra > roomInfo.MaxExtraOccupancy)
    //                throw new InvalidOperationException(
    //                    $"Extra occupancy {extra} exceeds allowed {roomInfo.MaxExtraOccupancy}");

    //            if (!chargesDict.TryGetValue((r.RoomId, extra), out var nightlyCharge))
    //                throw new InvalidOperationException(
    //                    $"No charge configured for Room={r.RoomId}, Extra={extra}");

    //            var subtotal = nightlyCharge * nights;
    //            roomsAmount += subtotal;

    //            reservationRooms.Add(new ReservationRoom
    //            {
    //                RoomId = r.RoomId,
    //                FromDate = r.FromDate,
    //                ToDate = r.ToDate,
    //                Subtotal = subtotal,
    //                PricePerNight = nightlyCharge
    //            });
    //        }

    //        if (roomsAmount != cmd.PaidAmount)
    //            throw new InvalidOperationException("Payment amount mismatch");

    //        // 2️⃣ CREATE RESERVATION (DIRECTLY CONVERTED)
    //        var reservation = new Reservation
    //        {
    //            UserId = currentUserId,
    //            ClubId = cmd.ClubId,
    //            Status = ReservationStatus.Converted,
    //            RoomsAmount = roomsAmount,
    //            TotalAmount = roomsAmount,
    //            AmountPaidSoFar = cmd.PaidAmount
    //        };

    //        foreach (var rr in reservationRooms)
    //            reservation.ReservationRooms.Add(rr);

    //        _context.Reservations.Add(reservation);

    //        // 3️⃣ CREATE BOOKINGS (CONFIRMED)
    //        foreach (var rr in reservationRooms)
    //        {
    //            _context.RoomBookings.Add(new Domain.Entities.RoomBooking
    //            {
    //                Reservation = reservation,
    //                UserId = currentUserId,
    //                ClubId = cmd.ClubId,
    //                RoomId = rr.RoomId,
    //                Status = BookingStatus.Confirmed,
    //                CheckInDate = rr.FromDate,
    //                CheckOutDate = rr.ToDate,
    //                TotalAmount = rr.Subtotal
    //            });
    //        }

    //        // 4️⃣ PAYMENT INTENT
    //        var intent = new PaymentIntent
    //        {
    //            Reservation = reservation,
    //            AmountToCollect = cmd.PaidAmount,
    //            Status = PaymentIntentStatus.Succeeded,
    //            Provider = cmd.Provider,
    //            Method = cmd.Method,
    //            ProviderIntentId = cmd.ProviderIntentId
    //        };

    //        _context.PaymentIntents.Add(intent);

    //        // 5️⃣ PAYMENT
    //        _context.Payments.Add(new Payment
    //        {
    //            PaymentIntent = intent,
    //            Amount = cmd.PaidAmount,
    //            Status = PaymentStatus.Paid,
    //            Provider = cmd.Provider,
    //            Method = cmd.Method,
    //            ProviderTransactionId = cmd.ProviderTransactionId,
    //            PaidAt = DateTime.Now,
    //            RawResponse = cmd.RawResponse
    //        });

    //        await _context.SaveChangesAsync(ct);
    //        await tx.CommitAsync(ct);

    //        return reservation.Id;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.ToString());
    //        await tx.RollbackAsync(ct);
    //        throw; // 🚨 THIS IS MANDATORY
    //    }
    //}
}

