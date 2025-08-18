using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationDashboard;
public record GetReservationDashboardQuery : IRequest<ReservationDashboardDto>;

public class GetReservationDashboardHandler
    : IRequestHandler<GetReservationDashboardQuery, ReservationDashboardDto>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetReservationDashboardHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReservationDashboardDto> Handle(GetReservationDashboardQuery request, CancellationToken ct)
    {
        var reservations = await _context.Reservations
            .Include(r => r.Club)
            .Include(r => r.PaymentIntents)
                .ThenInclude(pi => pi.Payments)
            .ToListAsync(ct);

        int totalReservations = reservations.Count;
        int activeReservations = reservations.Count(r =>
            r.Status == DHAFacilitationAPIs.Domain.Enums.ReservationStatus.AwaitingPayment ||
            r.Status == DHAFacilitationAPIs.Domain.Enums.ReservationStatus.Converted);

        int cancelledReservations = reservations.Count(r =>
            r.Status == DHAFacilitationAPIs.Domain.Enums.ReservationStatus.Cancelled ||
            r.Status == DHAFacilitationAPIs.Domain.Enums.ReservationStatus.Expired);

        int completedReservations = reservations.Count(r =>
            r.Status == DHAFacilitationAPIs.Domain.Enums.ReservationStatus.Converted);

        decimal totalRevenue = reservations.Sum(r => r.TotalAmount);
        decimal totalPaid = reservations.Sum(r =>
            r.PaymentIntents?.SelectMany(pi => pi.Payments)
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount) ?? 0);

        decimal pendingAmount = totalRevenue - totalPaid;

        // ✅ Grouping by Club safely
        var byClub = reservations
            .GroupBy(r => r.Club?.Name ?? "Unknown Club")
            .ToDictionary(g => g.Key, g => g.Count());

        var byStatus = reservations
            .GroupBy(r => r.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // ✅ Stage-wise summary
        int reservationStageApproved = 0;
        int reservationStagePending = 0;
        int paymentStageApproved = 0;
        int paymentStagePending = 0;
        int bookingStageApproved = 0;
        int bookingStagePending = 0;

        foreach (var r in reservations)
        {
            decimal paid = r.PaymentIntents?.SelectMany(pi => pi.Payments)
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount) ?? 0;

            // Reservation Stage
            if (r.ExpiresAt > DateTime.Now) reservationStageApproved++;
            else reservationStagePending++;

            // Payment Stage
            if (paid >= r.TotalAmount) paymentStageApproved++;
            else paymentStagePending++;

            // Booking Stage (only confirmed after payment)
            if (paid >= r.TotalAmount) bookingStageApproved++;
            else bookingStagePending++;
        }

        return new ReservationDashboardDto
        {
            TotalReservations = totalReservations,
            ActiveReservations = activeReservations,
            CancelledReservations = cancelledReservations,
            CompletedReservations = completedReservations,
            TotalRevenue = totalRevenue,
            TotalPaid = totalPaid,
            PendingAmount = pendingAmount,
            ReservationsByClub = byClub,
            ReservationsByStatus = byStatus,
            ReservationStageApproved = reservationStageApproved,
            ReservationStagePending = reservationStagePending,
            PaymentStageApproved = paymentStageApproved,
            PaymentStagePending = paymentStagePending,
            BookingStageApproved = bookingStageApproved,
            BookingStagePending = bookingStagePending
        };
    }

}

