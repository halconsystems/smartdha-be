using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DHAFacilitationAPIs.Application.Feature.Refund.Command.UpdateRefundRequest;

public class UpdateRefundRequestCommand : IRequest<string>
{
    public Guid RefundRequestId { get; set; }
    public RefundStatus Status { get; set; }   // Approved / Rejected
    public string? Notes { get; set; }
}

public class UpdateRefundRequestCommandHandler
    : IRequestHandler<UpdateRefundRequestCommand, string>
{
    private readonly IOLMRSApplicationDbContext _context;

    public UpdateRefundRequestCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateRefundRequestCommand request, CancellationToken cancellationToken)
    {
        if (request.RefundRequestId == Guid.Empty)
            throw new ArgumentException("RefundRequestId must be provided.");

        var refundRequest = await _context.RefundRequests
            .FirstOrDefaultAsync(rr => rr.Id == request.RefundRequestId, cancellationToken);

        if (refundRequest == null)
            throw new NotFoundException(nameof(RefundRequest), request.RefundRequestId.ToString());

        if (refundRequest.Status != RefundStatus.Pending)
            throw new InvalidOperationException("Only pending refund requests can be updated.");

        refundRequest.Status = request.Status;
        refundRequest.Notes = request.Notes ?? refundRequest.Notes;

        _context.RefundRequests.Update(refundRequest);

        string msg1 = string.Empty;
        string msg2 = string.Empty;
        string msg3 = "Refund request updated successfully.";

        // If refund approved, cascade cancellation
        if (request.Status == RefundStatus.Approved)
        {
            // Find reservation
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == refundRequest.ReservationId, cancellationToken);

            if (reservation == null)
                throw new NotFoundException(nameof(Reservation), refundRequest.ReservationId.ToString());

            if (reservation.Status == ReservationStatus.Cancelled || reservation.Status == ReservationStatus.Expired)
                throw new InvalidOperationException("Refund cannot cancel an already cancelled or expired reservation.");

            reservation.Status = ReservationStatus.Cancelled;
            _context.Reservations.Update(reservation);

            msg1 = "Reservation status updated to cancelled.";

            // Cancel all linked room bookings
            var roomBookings = await _context.RoomBookings
                .Where(rb => rb.ReservationId == reservation.Id)
                .ToListAsync(cancellationToken);

            if (!roomBookings.Any())
                throw new InvalidOperationException($"No room bookings found for reservation {reservation.Id}.");

            foreach (var booking in roomBookings)
            {
                booking.Status = BookingStatus.Cancelled;
            }

            _context.RoomBookings.UpdateRange(roomBookings);

            msg2 = "RoomBooking statuses updated to cancelled.";

        }

        await _context.SaveChangesAsync(cancellationToken);

        var responseMessage = string.Join(" | ", new[] { msg1, msg2, msg3 }.Where(m => !string.IsNullOrEmpty(m)));
        return responseMessage; 
    }
}
