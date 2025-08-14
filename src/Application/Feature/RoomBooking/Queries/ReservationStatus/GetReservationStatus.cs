using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus;
public record GetReservationStatusQuery(Guid ReservationId) : IRequest<ReservationStatusDto>;

public class GetReservationStatusQueryHandler : IRequestHandler<GetReservationStatusQuery, ReservationStatusDto>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetReservationStatusQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReservationStatusDto> Handle(GetReservationStatusQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _context.Reservations
            .Include(r => r.PaymentIntents)
                .ThenInclude(pi => pi.Payments)
            .FirstOrDefaultAsync(r => r.Id == request.ReservationId, cancellationToken);

        if (reservation == null)
            throw new Exception($"Reservation {request.ReservationId} not found");

        // 1️⃣ Reservation Stage
        var reservationStage = new StageStatus
        {
            Status = reservation.Status.ToString(),
            StatusCode = (int)reservation.Status
        };

        // 2️⃣ Payment Stage — latest payment for this reservation
        var latestPayment = reservation.PaymentIntents
            .SelectMany(pi => pi.Payments)
            .OrderByDescending(p => p.Created)
            .FirstOrDefault();

        var paymentStage = latestPayment != null
            ? new StageStatus
            {
                Status = latestPayment.Status.ToString(),
                StatusCode = (int)latestPayment.Status
            }
            : new StageStatus { Status = null, StatusCode = null };

        // 3️⃣ Booking Stage — get from RoomBooking table if exists
        var booking = await _context.RoomBookings
            .FirstOrDefaultAsync(b => b.ReservationId == request.ReservationId, cancellationToken);

        var bookingStage = booking != null
            ? new StageStatus
            {
                Status = booking.Status.ToString(), 
                StatusCode = (int)booking.Status
            }
            : new StageStatus { Status = null, StatusCode = null };

        return new ReservationStatusDto
        {
            ReservationStage = reservationStage,
            PaymentStage = paymentStage,
            BookingStage = bookingStage
        };
    }
}

