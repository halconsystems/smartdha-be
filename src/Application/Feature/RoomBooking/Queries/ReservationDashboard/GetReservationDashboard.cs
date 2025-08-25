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
            .Include(r => r.ReservationRooms)
            .Include(r => r.PaymentIntents)
                .ThenInclude(pi => pi.Payments)
            .ToListAsync(ct);

        var reservationStatuses = reservations.Select(r =>
        {
            decimal paid = r.PaymentIntents?
                .SelectMany(pi => pi.Payments)
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount) ?? 0;

            return new
            {
                Reservation = r,
                Paid = paid,
                DisplayStatus = GetDisplayStatus(r, paid)
            };
        }).ToList();

        int totalReservations = reservationStatuses.Count;
        int confirmedReservations = reservationStatuses.Count(x => x.DisplayStatus == "Booking Confirmed");
        int cancelledReservations = reservationStatuses.Count(x => x.DisplayStatus == "Cancelled");
        int awaitingPaymentReservations = reservationStatuses.Count(x => x.DisplayStatus == "Awaiting Payment");

        // Active = confirmed + awaiting payment
        int activeReservations = confirmedReservations + awaitingPaymentReservations;

        // ✅ Payments & Revenue
        decimal totalPaid = reservationStatuses.Sum(x => x.Paid);
        decimal totalRevenue = totalPaid;

        // ✅ Club-wise stats
        var clubStats = reservationStatuses
            .GroupBy(x => x.Reservation.Club?.Name ?? "Unknown Club")
            .Select(g => new ClubDashboardDto
            {
                ClubName = g.Key,
                TotalReservations = g.Count(),
                ConfirmedReservations = g.Count(r => r.DisplayStatus == "Booking Confirmed"),
                CancelledReservations = g.Count(r => r.DisplayStatus == "Cancelled"),
                AwaitingPaymentReservations = g.Count(r => r.DisplayStatus == "Awaiting Payment"),
                TotalRoomsBooked = g.Where(r => r.DisplayStatus != "Cancelled")
                                    .SelectMany(r => r.Reservation.ReservationRooms).Count(),
                ConfirmationPercentage = g.Any()
                    ? Math.Round((decimal)g.Count(r => r.DisplayStatus == "Booking Confirmed") / g.Count() * 100, 2)
                    : 0
            })
            .ToList();

        // ✅ Stage progress (Approved + Pending + Cancelled = TotalReservations)
        int reservationStageApproved = confirmedReservations;
        int reservationStagePending = awaitingPaymentReservations;

        // Cancelled is tracked separately
        int paymentStageApproved = confirmedReservations; // paid fully
        int paymentStagePending = awaitingPaymentReservations; // awaiting only

        int bookingStageApproved = confirmedReservations;
        int bookingStagePending = awaitingPaymentReservations;

        return new ReservationDashboardDto
        {
            TotalReservations = totalReservations,
            ActiveReservations = activeReservations,
            CancelledReservations = cancelledReservations,
            CompletedReservations = confirmedReservations,
            TotalRevenue = totalRevenue,
            TotalPaid = totalPaid,

            AwaitingPaymentReservations = awaitingPaymentReservations,
            Clubs = clubStats,

            ReservationStageApproved = reservationStageApproved,
            ReservationStagePending = reservationStagePending,
            PaymentStageApproved = paymentStageApproved,
            PaymentStagePending = paymentStagePending,
            BookingStageApproved = bookingStageApproved,
            BookingStagePending = bookingStagePending
        };
    }

    private string GetDisplayStatus(Reservation r, decimal paid)
    {
        if (paid >= r.TotalAmount)
            return "Booking Confirmed";

        if (r.ExpiresAt <= DateTime.Now)
            return "Cancelled";

        return "Awaiting Payment";
    }





}

