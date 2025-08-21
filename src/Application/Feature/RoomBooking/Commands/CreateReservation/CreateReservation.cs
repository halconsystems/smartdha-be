using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation.Dtos;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;

public record class CreateReservationCommand : IRequest<ReservationInfoDto>
{
    public Guid ClubId { get; set; }
    public RoomBookingType BookingType { get; set; } = RoomBookingType.Self;
    public int Adult { get; set; } = 0;
    public int Child { get; set; } = 0;
    public List<CreateReservationRoomDto> Rooms { get; set; } = new();
    public CreateGuestDto? Guest { get; set; }
}


public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ReservationInfoDto>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateReservationCommandHandler(IOLMRSApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ReservationInfoDto> Handle(CreateReservationCommand dto, CancellationToken cancellationToken)
    {
        //var dto = request.Reservation;

        var _currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(_currentUserId))
            throw new UnauthorizedAccessException("Invalid or missing UserId in token.");

        if (dto.Rooms == null || !dto.Rooms.Any())
            throw new InvalidOperationException("At least one room must be provided.");

        // Calculate totals
        //decimal roomsAmount = dto.Rooms.Sum(r =>
        //{
        //    var totalDays = (r.ToDate.Date - r.FromDate.Date).Days;
        //    if (totalDays <= 0)
        //        throw new Exception("ToDate must be after FromDate");

        //    return totalDays * r.PricePerNight;
        //});
        // Step 1: Load charges for all rooms in the DTO
        var roomIds = dto.Rooms.Select(r => r.RoomId).ToList();

        var chargesDict = await _context.RoomCharges
            .Where(c => roomIds.Contains(c.RoomId))
            .ToDictionaryAsync(c => (c.RoomId, c.BookingType), c => c.Charges, cancellationToken);

        // Step 2: Calculate total amount
        decimal roomsAmount = dto.Rooms.Sum(r =>
        {
            var totalDays = (r.ToDate.Date - r.FromDate.Date).Days;
            if (totalDays <= 0)
                throw new Exception("ToDate must be after FromDate");

            //// fallback: if no charge found, you can decide to throw or default
            if (!chargesDict.TryGetValue((r.RoomId, dto.BookingType), out var roomCharge))
                throw new Exception($"No charges configured for RoomId {r.RoomId} with BookingType {dto.BookingType}");

            return totalDays * roomCharge;
        });




        decimal taxes = 0m;
        decimal discounts = 0m;
        decimal totalAmount = roomsAmount + taxes - discounts;

        decimal depositPercent = 30m;
        decimal depositAmount = Math.Round(totalAmount * (depositPercent / 100m), 2);


        string onebillId = GenerateOneBillId();

        var reservation = new Reservation
        {
            UserId = new Guid(_currentUserId),
            ClubId = dto.ClubId,
            Status = ReservationStatus.AwaitingPayment,
            ExpiresAt = DateTime.Now.AddMinutes(30),
            RoomsAmount = roomsAmount,
            Taxes = taxes,
            Discounts = discounts,
            TotalAmount = totalAmount,
            DepositPercentRequired = depositPercent,
            DepositAmountRequired = depositAmount,
            Adult=dto.Adult,
            Child=dto.Child,
            OneBillId = onebillId
        };

        // Check Guest CNIC if guest data provided
        if (dto.Guest != null && !string.IsNullOrWhiteSpace(dto.Guest.CNICOrPassport) && dto.BookingType!=RoomBookingType.Self)
        {
            var existingGuest = await _context.BookingGuests
                .FirstOrDefaultAsync(g => g.CNICOrPassport == dto.Guest.CNICOrPassport, cancellationToken);

            if (existingGuest != null)
            {
                reservation.GuestId = existingGuest.Id;
            }
            else
            {
                // No match → create new guest
                var newGuest = new BookingGuest
                {
                    FullName = dto.Guest.FullName,
                    CNICOrPassport = dto.Guest.CNICOrPassport,
                    Phone = dto.Guest.Phone,
                    Email = dto.Guest.Email,
                    Address = dto.Guest.Address
                };

                _context.BookingGuests.Add(newGuest);
                await _context.SaveChangesAsync(cancellationToken); // Save immediately to get ID

                reservation.GuestId = newGuest.Id;
            }
        }

        // Add reservation rooms
        foreach (var room in dto.Rooms)
        {
            var totalDays = (room.ToDate.Date - room.FromDate.Date).Days;
            if (totalDays <= 0)
                throw new Exception("ToDate must be after FromDate");

            decimal PricePerNight=0;

            var roomCharges = await _context.RoomCharges
           .Where(c => c.RoomId == room.RoomId).FirstOrDefaultAsync();
            if(roomCharges != null)
            {
                PricePerNight = roomCharges.Charges;
            }

            var subTotal = totalDays * PricePerNight;
            reservation.ReservationRooms.Add(new ReservationRoom
            {
                RoomId = room.RoomId,
                FromDate = room.FromDate,
                ToDate = room.ToDate,
                PricePerNight = PricePerNight,
                Subtotal = subTotal
            });
        }

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        var getClub = await _context.Clubs
        .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Id == dto.ClubId, cancellationToken);

        ClubInfoDto clubInfoDto = new ClubInfoDto();


        if (getClub == null)
            throw new Exception($"Club with Id {dto.ClubId} not found");
        else
        {
            clubInfoDto.Name = getClub.Name;
            clubInfoDto.Location = getClub.Location;
            clubInfoDto.ContactNumber = getClub.ContactNumber;
        }

        var roomreservation = await _context.Reservations
     .AsNoTracking()
     .Include(r => r.ReservationRooms)
         .ThenInclude(rr => rr.Room)
             .ThenInclude(r => r.RoomCategory)
     .Include(r => r.ReservationRooms)
         .ThenInclude(rr => rr.Room)
             .ThenInclude(r => r.ResidenceType)
     .Include(r => r.ReservationRooms)
         .ThenInclude(rr => rr.Room)
             .ThenInclude(r => r.RoomServices)
     .FirstOrDefaultAsync(r => r.Id == reservation.Id, cancellationToken);

        if (roomreservation == null)
            throw new Exception($"Reservation {reservation.Id} not found");

        // Build DTO to return
        var reservationInfo = new ReservationInfoDto
        {
            OneBillId = roomreservation.OneBillId,
            ExpiresAt = roomreservation.ExpiresAt,
            Status = roomreservation.Status.ToString(),
            Club = clubInfoDto,  // I assume you already built this separately
            Rooms = roomreservation.ReservationRooms.Select(rr => new ReservationRoomInfoDto
            {
                RoomNo = rr.Room?.No ?? string.Empty,
                RoomName = rr.Room?.Name ?? string.Empty,
                Description = rr.Room?.Description ?? string.Empty,
                CategoryName = rr.Room?.RoomCategory?.Name ?? string.Empty,
                ResidenceType = rr.Room?.ResidenceType?.Name ?? string.Empty,
                //Services = rr.Room?.RoomServices?.Select(s => s.Name).ToList() ?? new List<string>(),
                Services = _context.ServiceMappings
                 .Where(sm => sm.RoomId == rr.RoomId && (sm.IsDeleted == false || sm.IsDeleted == null))
                 .Select(sm => sm.Services.Name)
                  .ToList(),

                FromDate = rr.FromDate,
                ToDate = rr.ToDate,
                TotalNights = (rr.ToDate.Date - rr.FromDate.Date).Days,
                PricePerNight = rr.PricePerNight,
                Subtotal = rr.Subtotal
            }).ToList()
        };

        return reservationInfo;

    }

    public static string GenerateOneBillId()
    {
        // Base = yyyyMMddHHmm (12 digits, up to minutes)
        string baseId = DateTime.UtcNow.ToString("yyyyMMddHHmm");

        // Add 3 random digits → total length = 15
        string random = new Random().Next(100, 999).ToString();

        string final = baseId + random; // 15 digits

        // Ensure length is between 10 and 15
        if (final.Length > 15)
            final = final.Substring(0, 15);
        else if (final.Length < 10)
            final = final.PadRight(10, '0');

        return final;
    }

}
