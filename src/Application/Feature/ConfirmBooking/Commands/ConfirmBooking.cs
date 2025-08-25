using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ConfirmBooking.Commands;
public class ConfirmBookingCommand : IRequest<SuccessResponse<BookingConfirmationDto>>
{

    [Required] public Guid ReservationId { get; set; }
    [Required] public string ProviderTransactionId { get; set; } = string.Empty;
    [Required] public decimal Amount { get; set; }
    [Required] public PaymentMethod Method { get; set; }
    [Required] public PaymentProvider Provider { get; set; }
}
public class ConfirmBookingCommandHandler
    : IRequestHandler<ConfirmBookingCommand, SuccessResponse<BookingConfirmationDto>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public ConfirmBookingCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<BookingConfirmationDto>> Handle(ConfirmBookingCommand request, CancellationToken ct)
    {
        // 1️⃣ Find Reservation with PaymentIntents
        var reservation = await _context.Reservations
            .Include(r => r.ReservationRooms)
            .Include(r => r.PaymentIntents)
                .ThenInclude(pi => pi.Payments)
            .FirstOrDefaultAsync(r => r.Id == request.ReservationId, ct);

        if (reservation == null)
            throw new Exception("Reservation not found");

        // 2️⃣ Use (or create) PaymentIntent
        var intent = reservation.PaymentIntents.FirstOrDefault();
        if (intent == null)
        {
            intent = new PaymentIntent
            {
                ReservationId = reservation.Id,
                AmountToCollect = reservation.TotalAmount,
                IsDeposit = false,
                Status = PaymentIntentStatus.RequiresPayment,
                Method = request.Method,
                Provider = request.Provider
            };
            await _context.PaymentIntents.AddAsync(intent, ct);
        }

        // 3️⃣ Create Payment
        var payment = new Payment
        {
            PaymentIntentId = intent.Id,
            Amount = request.Amount,
            Status = PaymentStatus.Paid,
            PaidAt = DateTime.Now,
            Method = request.Method,
            Provider = request.Provider,
            ProviderTransactionId = request.ProviderTransactionId // you may map actual txnId
        };
        intent.Payments.Add(payment);

        // 4️⃣ Update PaymentIntent status
        var totalPaid = intent.Payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount);
        if (totalPaid >= reservation.TotalAmount)
        {
            intent.Status = PaymentIntentStatus.Succeeded;
            reservation.Status = ReservationStatus.Converted;
        }
        else
        {
            intent.Status = DHAFacilitationAPIs.Domain.Enums.PaymentIntentStatus.Partial;
            reservation.Status = ReservationStatus.AwaitingPayment;
        }

        // 5️⃣ Create RoomBooking(s) only if fully paid
        List<DHAFacilitationAPIs.Domain.Entities.RoomBooking> bookings = new();
        if (reservation.Status == ReservationStatus.Converted)
        {
            foreach (var rr in reservation.ReservationRooms)
            {
                var booking = new DHAFacilitationAPIs.Domain.Entities.RoomBooking
                {
                    UserId = reservation.UserId,
                    ClubId = reservation.ClubId,
                    RoomId = rr.RoomId,
                    ReservationId = reservation.Id,
                    BookingDate = DateTime.Now,
                    CheckInDate = rr.FromDate,
                    CheckOutDate = rr.ToDate,
                    TotalAmount = rr.Subtotal,
                    Status = BookingStatus.Confirmed
                };
                bookings.Add(booking);
                await _context.RoomBookings.AddAsync(booking, ct);
            }
        }

        await _context.SaveChangesAsync(ct);

        // 6️⃣ Build response DTO
        var confirmation = new BookingConfirmationDto
        {
            ReservationId = reservation.Id,
            Status = reservation.Status.ToString(),
            PaidAmount = totalPaid,
            TotalAmount = reservation.TotalAmount,
            PaymentStatus = intent.Status.ToString(),
            Bookings = bookings.Select(b => new BookingDetailDto
            {
                BookingId = b.Id,
                RoomId = b.RoomId,
                CheckIn = b.CheckInDate,
                CheckOut = b.CheckOutDate,
                TotalAmount = b.TotalAmount,
                Status = b.Status.ToString()
            }).ToList()
        };

        return new SuccessResponse<BookingConfirmationDto>(confirmation, "Booking confirmed successfully");
    }
}

