using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Contracts;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation.Dtos;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
public record class CreateReservationAdminCommand : IRequest<ReservationInfoDto>
{
    public Guid UserId { get; set; }                  // 👈 the user to book for
    public Guid ClubId { get; set; }
    public RoomBookingType BookingType { get; set; } = RoomBookingType.Self;
    public List<CreateReservationRoomDto> Rooms { get; set; } = new();
    public CreateGuestDto? Guest { get; set; }
}
public class CreateReservationAdminCommandHandler
    : IRequestHandler<CreateReservationAdminCommand, ReservationInfoDto>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateReservationAdminCommandHandler(
        IOLMRSApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ReservationInfoDto> Handle(CreateReservationAdminCommand dto, CancellationToken ct)
    {
        //// ✅ ensure only admins can access
        //var isAdmin = _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
        //if (!isAdmin)
        //    throw new UnauthorizedAccessException("Only admins can create bookings for other users.");

        var clubType = await _context.Clubs
                .Where(c => c.Id == dto.ClubId && c.IsDeleted == false && c.IsActive == true)
                .Select(c => (ClubType ?)c.ClubType).FirstOrDefaultAsync();

        if (clubType == null)
            throw new Exception("Club not found");

        if (dto.Rooms == null || !dto.Rooms.Any())
            throw new InvalidOperationException("At least one room must be provided.");

        // 🔽 collect requested roomIds
        var roomIds = dto.Rooms.Select(x => x.RoomId).Distinct().ToList();

        // 🔽 load rooms with occupancy
        var roomsMap = await _context.Rooms
            .AsNoTracking()
            .Where(r => roomIds.Contains(r.Id))
            .Select(r => new { r.Id, r.ClubId, r.NormalOccupancy, r.MaxExtraOccupancy })
            .ToDictionaryAsync(r => r.Id, r => r, ct);

        // validate rooms belong to club
        if (roomsMap.Values.Any(r => r.ClubId != dto.ClubId))
            throw new InvalidOperationException("One or more rooms do not belong to the specified club.");

        // 🔽 load charges
        var chargesDict = await _context.RoomCharges
            .Where(c => roomIds.Contains(c.RoomId) && c.BookingType == dto.BookingType)
            .ToDictionaryAsync(c => (c.RoomId, c.BookingType, c.ExtraOccupancy), c => c.Charges, ct);

        // 🔽 overlap guard
        foreach (var rr in dto.Rooms)
        {
            //var reqFromDO = DateOnly.FromDateTime(rr.FromDate);
            //var reqToDO = DateOnly.FromDateTime(rr.ToDate);

            var conflict = await _context.ReservationRooms
                .AsNoTracking()
                .AnyAsync(x =>
                    x.RoomId == rr.RoomId &&
                    x.FromDate <= rr.ToDate &&
                    x.ToDate >= rr.FromDate &&
                    (x.Reservation.Status == ReservationStatus.AwaitingPayment ||
                     x.Reservation.Status == ReservationStatus.Converted),
                    ct);

            if (conflict)
                throw new InvalidOperationException("Selected room is already reserved in the requested dates.");
        }

        // 🔽 price calc
        decimal roomsAmount = 0m;
        int totalAdults = 0, totalChildren = 0;
        var reservationRooms = new List<ReservationRoom>();

        foreach (var r in dto.Rooms)
        {
            decimal nights = 0;

            if (clubType == ClubType.GuestRoom)
            {
                nights = (r.ToDate.Date - r.FromDate.Date).Days;
            }
            else if (clubType == ClubType.Ground)
            {
                nights = (decimal)(r.ToDate - r.FromDate).TotalHours;
            }

            if (nights <= 0)
                throw new InvalidOperationException("ToDate must be after FromDate.");

            if (!roomsMap.TryGetValue(r.RoomId, out var roomInfo))
                throw new InvalidOperationException($"Room not found: {r.RoomId}");

            var occupants = r.Adults + r.Children;
            var extra = Math.Max(0, occupants - roomInfo.NormalOccupancy);
            if (extra > roomInfo.MaxExtraOccupancy)
                throw new InvalidOperationException($"Extra occupancy {extra} exceeds allowed {roomInfo.MaxExtraOccupancy} for room {r.RoomId}.");

            if (!chargesDict.TryGetValue((r.RoomId, dto.BookingType, extra), out var nightlyTotal))
                throw new InvalidOperationException($"No RoomCharge configured for RoomId={r.RoomId}, BookingType={dto.BookingType}, ExtraOccupancy={extra}.");

            var subTotal = nightlyTotal * nights;

            roomsAmount += subTotal;
            totalAdults += r.Adults;
            totalChildren += r.Children;

            reservationRooms.Add(new ReservationRoom
            {
                RoomId = r.RoomId,
                FromDate = r.FromDate,
                ToDate = r.ToDate,
                FromDateOnly = DateOnly.FromDateTime(r.FromDate),
                ToDateOnly = DateOnly.FromDateTime(r.ToDate),
                FromTimeOnly = TimeOnly.FromDateTime(r.FromDate),
                ToTimeOnly = TimeOnly.FromDateTime(r.ToDate),
                PricePerNight = nightlyTotal,
                Subtotal = subTotal
            });
        }

        decimal taxes = 0m;
        decimal discounts = 0m;
        decimal totalAmount = roomsAmount + taxes - discounts;
        decimal depositPercent = 30m;
        decimal depositAmount = Math.Round(totalAmount * (depositPercent / 100m), 2);

        var club = await _context.Clubs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.ClubId, ct)
            ?? throw new InvalidOperationException($"Club with Id {dto.ClubId} not found.");

        if (string.IsNullOrWhiteSpace(club.AccountNoAccronym))
            throw new Exception($"Club {club.Id} has no valid AccountNoAccronym configured.");

        string onebillId = SmartPayBillId.GenerateOneBillId(club.AccountNoAccronym);

        var reservation = new Reservation
        {
            UserId = dto.UserId,  // 👈 direct user assignment
            ClubId = dto.ClubId,
            Status = ReservationStatus.AwaitingPayment,
            ExpiresAt = DateTime.Now.AddMinutes(30),
            RoomsAmount = roomsAmount,
            Taxes = taxes,
            Discounts = discounts,
            TotalAmount = totalAmount,
            DepositPercentRequired = depositPercent,
            DepositAmountRequired = depositAmount,
            Adult = totalAdults,
            Child = totalChildren,
            OneBillId = onebillId
        };

        // guest handling (same as before)
        if (dto.Guest != null &&
            !string.IsNullOrWhiteSpace(dto.Guest.CNICOrPassport) &&
            dto.BookingType != RoomBookingType.Self)
        {
            var existingGuest = await _context.BookingGuests
                .FirstOrDefaultAsync(g => g.CNICOrPassport == dto.Guest.CNICOrPassport, ct);

            if (existingGuest != null)
            {
                reservation.GuestId = existingGuest.Id;
            }
            else
            {
                var newGuest = new BookingGuest
                {
                    FullName = dto.Guest.FullName,
                    CNICOrPassport = dto.Guest.CNICOrPassport,
                    Phone = dto.Guest.Phone,
                    Email = dto.Guest.Email,
                    Address = dto.Guest.Address
                };
                _context.BookingGuests.Add(newGuest);
                await _context.SaveChangesAsync(ct);
                reservation.GuestId = newGuest.Id;
            }
        }

        foreach (var rr in reservationRooms)
            reservation.ReservationRooms.Add(rr);

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(ct);

        // 🔽 build response DTO
        var persisted = await _context.Reservations
            .AsNoTracking()
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                    .ThenInclude(room => room.RoomCategory)
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                    .ThenInclude(room => room.ResidenceType)
            .FirstOrDefaultAsync(r => r.Id == reservation.Id, ct)
            ?? throw new InvalidOperationException($"Reservation {reservation.Id} not found after save.");

        return new ReservationInfoDto
        {
            OneBillId = persisted.OneBillId,
            ExpiresAt = persisted.ExpiresAt,
            Status = persisted.Status.ToString(),
            Club = new ClubInfoDto
            {
                Name = club.Name,
                Location = club.Location,
                ContactNumber = club.ContactNumber
            },
            Rooms = persisted.ReservationRooms.Select(rr => new ReservationRoomInfoDto
            {
                RoomNo = rr.Room?.No ?? string.Empty,
                RoomName = rr.Room?.Name,
                Description = rr.Room?.Description,
                CategoryName = rr.Room?.RoomCategory?.Name ?? string.Empty,
                ResidenceType = rr.Room?.ResidenceType?.Name ?? string.Empty,
                Services = _context.ServiceMappings
                    .Where(sm => sm.RoomId == rr.RoomId && (sm.IsDeleted == false || sm.IsDeleted == null))
                    .Select(sm => sm.Services.Name)
                    .ToList(),
                FromDate = rr.FromDate,
                ToDate = rr.ToDate,
                TotalNights = clubType == ClubType.GuestRoom
                    ? (decimal)(rr.ToDate.Date - rr.FromDate.Date).Days        // Whole days
                    : (decimal)(rr.ToDate - rr.FromDate).TotalHours,           // Fractional hours
                //TotalNights = (rr.ToDate.Date - rr.FromDate.Date).Days,
                PricePerNight = rr.PricePerNight,
                Subtotal = rr.Subtotal
            }).ToList()
        };
    }
}

