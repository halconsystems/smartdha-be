using System;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Room.Queries.GetAllRooms;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;

public record CreateReservationCommand(CreateReservationDto Reservation) : IRequest<Guid>;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IOLMRSApplicationDbContext _context;

    public CreateReservationCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Reservation;

        if (dto.UserId == Guid.Empty)
            throw new InvalidOperationException("UserId must be provided to create a reservation.");

        if (dto.Rooms == null || !dto.Rooms.Any())
            throw new InvalidOperationException("At least one room must be provided.");

        // Calculate totals
        decimal roomsAmount = dto.Rooms.Sum(r =>
        {
            var totalDays = (r.ToDate.Date - r.FromDate.Date).Days;
            if (totalDays <= 0)
                throw new Exception("ToDate must be after FromDate");

            return totalDays * r.PricePerNight;
        });
        decimal taxes = 0m;
        decimal discounts = 0m;
        decimal totalAmount = roomsAmount + taxes - discounts;

        decimal depositPercent = 30m;
        decimal depositAmount = Math.Round(totalAmount * (depositPercent / 100m), 2);

        var reservation = new Reservation
        {
            UserId = dto.UserId,
            ClubId = dto.ClubId,
            Status = ReservationStatus.AwaitingPayment,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            RoomsAmount = roomsAmount,
            Taxes = taxes,
            Discounts = discounts,
            TotalAmount = totalAmount,
            DepositPercentRequired = depositPercent,
            DepositAmountRequired = depositAmount
        };

        // Check Guest CNIC if guest data provided
        if (dto.Guest != null && !string.IsNullOrWhiteSpace(dto.Guest.CNICOrPassport))
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

            var subTotal = totalDays * room.PricePerNight;
            reservation.ReservationRooms.Add(new ReservationRoom
            {
                RoomId = room.RoomId,
                FromDate = room.FromDate,
                ToDate = room.ToDate,
                PricePerNight = room.PricePerNight,
                Subtotal = subTotal
            });
        }

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
