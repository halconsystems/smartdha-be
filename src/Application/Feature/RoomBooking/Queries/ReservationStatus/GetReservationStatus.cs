using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus;
public record GetReservationStatusQuery(Guid ReservationId) : IRequest<ReservationDetailDto>;

public class GetReservationStatusQueryHandler : IRequestHandler<GetReservationStatusQuery, ReservationDetailDto>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetReservationStatusQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReservationDetailDto> Handle(GetReservationStatusQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _context.Reservations
            .AsNoTracking()
            .Include(r => r.Club)
            .Include(r => r.Guest)
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                    .ThenInclude(r => r.RoomCategory)
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                    .ThenInclude(r => r.ResidenceType)
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                    .ThenInclude(r => r.RoomServices)
            .Include(r => r.PaymentIntents)
                .ThenInclude(pi => pi.Payments)
            .FirstOrDefaultAsync(r => r.Id == request.ReservationId, cancellationToken);

        if (reservation == null)
            throw new Exception($"Reservation {request.ReservationId} not found");

        decimal paid = reservation.PaymentIntents
        .SelectMany(pi => pi.Payments)
        .Where(p => p.Status == PaymentStatus.Paid)
        .Sum(p => p.Amount);


        var (reservationStage, paymentStage, bookingStage) = GetStages(reservation, paid);

        var displayStatus = GetDisplayStatus(reservation, paid);

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

        return new ReservationDetailDto
        {
            ReservationId = reservation.Id,
            OneBillId = reservation.OneBillId,
            Status = displayStatus,
            ExpiresAt = reservation.ExpiresAt,

            RoomsAmount = reservation.RoomsAmount,
            Taxes = reservation.Taxes,
            Discounts = reservation.Discounts,
            TotalAmount = reservation.TotalAmount,
            AmountPaidSoFar = reservation.AmountPaidSoFar,
            DepositPercentRequired = reservation.DepositPercentRequired,
            DepositAmountRequired = reservation.DepositAmountRequired,

            Adult = reservation.Adult,
            Child = reservation.Child,

            Guest = reservation.Guest == null ? null : new CreateGuestDto
            {
                FullName = reservation.Guest.FullName,
                CNICOrPassport = reservation.Guest.CNICOrPassport,
                Phone = reservation.Guest.Phone,
                Email = reservation.Guest.Email
            },

            Club = new ClubInfoDto
            {
                Name = reservation.Club.Name,
                Location = reservation.Club.Location,
                ContactNumber = reservation.Club.ContactNumber
            },

            Rooms = roomreservation.ReservationRooms.Select(rr => new ReservationRoomDetailDto
            {
                RoomId = rr.Room.Id,
                RoomNo = rr.Room.No,
                RoomName = rr.Room.Name,
                CategoryName = rr.Room.RoomCategory.Name,
                ResidenceType = rr.Room.ResidenceType.Name,
                //Services = rr.Room.RoomServices.Select(s => s.Name).ToList(),
                Services = _context.ServiceMappings
                 .Where(sm => sm.RoomId == rr.RoomId && (sm.IsDeleted == false || sm.IsDeleted == null))
                 .Select(sm => sm.Services.Name)
                  .ToList(),

                FromDate = rr.FromDate,
                ToDate = rr.ToDate,
                TotalNights = (rr.ToDate.Date - rr.FromDate.Date).Days,
                PricePerNight = rr.PricePerNight,
                Subtotal = rr.Subtotal
            }).ToList(),

            Payments = reservation.PaymentIntents.Select(pi => new PaymentIntentDto
            {
                PaymentIntentId = pi.Id,
                AmountToCollect = pi.AmountToCollect,
                IsDeposit = pi.IsDeposit,
                Status = pi.Status.ToString(),
                Method = pi.Method.ToString(),
                Provider = pi.Provider.ToString(),
                Payments = pi.Payments.Select(p => new PaymentDto
                {
                    PaymentId = p.Id,
                    Amount = p.Amount,
                    Status = p.Status.ToString(),
                    PaidAt = p.PaidAt,
                    Method = p.Method.ToString(),
                    Provider = p.Provider.ToString()
                }).ToList()
            }).ToList(),
            Reservationstage= reservationStage,
            Paymentstage=paymentStage,
            Bookingstage=bookingStage
        };
    }

    private string GetDisplayStatus(Reservation r, decimal paid)
    {
        if (paid >= r.TotalAmount)
            return "Booking Confirmed";

        if (r.ExpiresAt <= DateTime.Now)
            return "Cancelled"; // or "Expired"

        return "Awaiting Payment";
    }

    private (Reservationstage, Paymentstage, Bookingstage) GetStages(Reservation r, decimal paid)
    {
        var reservationStage = new Reservationstage();
        var paymentStage = new Paymentstage();
        var bookingStage = new Bookingstage();

        // Case 1: Fully paid
        if (paid >= r.TotalAmount)
        {
            reservationStage.Status = VerificationStatus.Approved;
            reservationStage.StatusCode = (int)VerificationStatus.Approved;

            paymentStage.Status = VerificationStatus.Approved;
            paymentStage.StatusCode = (int)VerificationStatus.Approved;

            bookingStage.Status = VerificationStatus.Pending;
            bookingStage.StatusCode = (int)VerificationStatus.Pending;

            return (reservationStage, paymentStage, bookingStage);
        }

        // Case 2: Expired without payment
        if (r.ExpiresAt <= DateTime.Now)
        {
            reservationStage.Status = VerificationStatus.Rejected;
            reservationStage.StatusCode = (int)VerificationStatus.Rejected;

            paymentStage.Status = VerificationStatus.Rejected;
            paymentStage.StatusCode = (int)VerificationStatus.Rejected;

            bookingStage.Status = VerificationStatus.Rejected;
            bookingStage.StatusCode = (int)VerificationStatus.Rejected;

            return (reservationStage, paymentStage, bookingStage);
        }

        // Case 3: Awaiting payment (still valid, unpaid or partial)
        reservationStage.Status = VerificationStatus.Approved;
        reservationStage.StatusCode = (int)VerificationStatus.Approved;

        paymentStage.Status = VerificationStatus.Pending;
        paymentStage.StatusCode = (int)VerificationStatus.Pending;

        bookingStage.Status = VerificationStatus.Pending;
        bookingStage.StatusCode = (int)VerificationStatus.Pending;

        return (reservationStage, paymentStage, bookingStage);
    }


    //public async Task<ReservationStatusDto> Handle(GetReservationStatusQuery request, CancellationToken cancellationToken)
    //{
    //    var reservation = await _context.Reservations
    //        .Include(r => r.PaymentIntents)
    //            .ThenInclude(pi => pi.Payments)
    //        .FirstOrDefaultAsync(r => r.Id == request.ReservationId, cancellationToken);

    //    if (reservation == null)
    //        throw new Exception($"Reservation {request.ReservationId} not found");

    //    // 1️⃣ Reservation Stage
    //    var reservationStage = new StageStatus
    //    {
    //        Status = reservation.Status.ToString(),
    //        StatusCode = (int)reservation.Status
    //    };

    //    // 2️⃣ Payment Stage — latest payment for this reservation
    //    var latestPayment = reservation.PaymentIntents
    //        .SelectMany(pi => pi.Payments)
    //        .OrderByDescending(p => p.Created)
    //        .FirstOrDefault();

    //    var paymentStage = latestPayment != null
    //        ? new StageStatus
    //        {
    //            Status = latestPayment.Status.ToString(),
    //            StatusCode = (int)latestPayment.Status
    //        }
    //        : new StageStatus { Status = null, StatusCode = null };

    //    // 3️⃣ Booking Stage — get from RoomBooking table if exists
    //    var booking = await _context.RoomBookings
    //        .FirstOrDefaultAsync(b => b.ReservationId == request.ReservationId, cancellationToken);

    //    var bookingStage = booking != null
    //        ? new StageStatus
    //        {
    //            Status = booking.Status.ToString(), 
    //            StatusCode = (int)booking.Status
    //        }
    //        : new StageStatus { Status = null, StatusCode = null };

    //    return new ReservationStatusDto
    //    {
    //        ReservationStage = reservationStage,
    //        PaymentStage = paymentStage,
    //        BookingStage = bookingStage
    //    };
    //}
}

