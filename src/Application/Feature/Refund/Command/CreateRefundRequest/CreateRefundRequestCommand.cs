using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Refund.Command.CreateRefundRequest;

public class CreateRefundRequestCommand : IRequest<Guid>
{
    public Guid ReservationId { get; set; }
    public string? Notes { get; set; }
}
public class CreateRefundRequestCommandHandler : IRequestHandler<CreateRefundRequestCommand, Guid>
{
    private readonly IOLMRSApplicationDbContext _context;

    public CreateRefundRequestCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateRefundRequestCommand request, CancellationToken cancellationToken)
    {
        if (request.ReservationId == Guid.Empty)
            throw new ArgumentException("ReservationId must be provided.");

        var reservation = await _context.Reservations
            .Include(r => r.ReservationRooms)
            .FirstOrDefaultAsync(r => r.Id == request.ReservationId, cancellationToken);

        if (reservation == null)
            throw new NotFoundException(nameof(Reservation), request.ReservationId.ToString());

        if (reservation.Status == ReservationStatus.Cancelled || reservation.Status == ReservationStatus.Expired)
            throw new InvalidOperationException("Refund cannot be requested for a cancelled or expired reservation.");

        var payments = await _context.Payments
            .Include(p => p.PaymentIntent)
            .Where(p => p.PaymentIntent.ReservationId == request.ReservationId && p.Status == PaymentStatus.Paid)
            .ToListAsync(cancellationToken);

        if (!payments.Any())
            throw new InvalidOperationException("No completed payments found for this reservation.");

        var amountPaid = payments.Sum(p => p.Amount);

        var checkInDate = reservation.ReservationRooms.Min(rr => rr.FromDate);
        var now = DateTime.UtcNow;

        var policy = await _context.RefundPolicies
            .Where(rp => rp.ClubId == reservation.ClubId
                      && rp.EffectiveFrom <= now
                      && rp.EffectiveTo >= now)
            .OrderByDescending(rp => rp.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (policy == null)
            throw new NotFoundException(nameof(RefundPolicy), $"No active refund policy found for ClubId {reservation.ClubId}");

        var hoursBeforeCheckIn = (checkInDate - now).TotalHours;
        decimal refundPercent = hoursBeforeCheckIn >= policy.HoursBeforeCheckIn ? policy.RefundPercent : 0m;

        var refundableAmount = amountPaid * (refundPercent / 100m);

        if (!policy.RefundDeposit)
        {
            refundableAmount -= reservation.DepositAmountRequired;
            if (refundableAmount < 0) refundableAmount = 0;
        }

        var refundRequest = new RefundRequest
        {
            ReservationId = reservation.Id,
            RequestedAt = now,
            RequestedAtDateOnly = DateOnly.FromDateTime(now),
            RequestedAtTimeOnly = TimeOnly.FromDateTime(now),
            AmountPaid = amountPaid,
            AmountRefunded = refundableAmount,
            Status = RefundStatus.Pending,
            Notes = request.Notes
        };

        _context.RefundRequests.Add(refundRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return refundRequest.Id;
    }
}
