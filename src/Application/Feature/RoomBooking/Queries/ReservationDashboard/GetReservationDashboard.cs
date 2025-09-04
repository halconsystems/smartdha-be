using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationDashboard;
public record GetReservationDashboardQuery : IRequest<ReservationDashboardDto>;

public class GetReservationDashboardHandler
    : IRequestHandler<GetReservationDashboardQuery, ReservationDashboardDto>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IApplicationDbContext _appCtx;
    private readonly ICurrentUserService _currentUser;

    public GetReservationDashboardHandler(
        IOLMRSApplicationDbContext context,
        IApplicationDbContext appCtx,
        ICurrentUserService currentUser)
    {
        _context = context;
        _appCtx = appCtx;
        _currentUser = currentUser;
    }


    public async Task<ReservationDashboardDto> Handle(GetReservationDashboardQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
            throw new UnAuthorizedException("Invalid user context.");

        // 1️⃣ Get current roles
        var roles = await _appCtx.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("SuperAdministrator");

        // 2️⃣ Base reservations query
        var reservationsQuery = _context.Reservations
            .Include(r => r.Club)
            .Include(r => r.ReservationRooms)
            .Include(r => r.PaymentIntents)
                .ThenInclude(pi => pi.Payments)
            .AsQueryable();

        // 3️⃣ Restrict to assigned clubs if not SuperAdmin
        if (!isSuperAdmin)
        {
            var assignedClubIds = await _appCtx.UserClubAssignments
                .Where(uca => uca.UserId == userId)
                .Select(uca => uca.ClubId)
                .ToListAsync(ct);

            reservationsQuery = reservationsQuery.Where(r => assignedClubIds.Contains(r.ClubId));
        }

        // 4️⃣ Load reservations
        var reservations = await reservationsQuery.ToListAsync(ct);

        // ✅ Map to dashboard stats
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

        int activeReservations = confirmedReservations + awaitingPaymentReservations;

        decimal totalPaid = reservationStatuses.Sum(x => x.Paid);
        decimal totalRevenue = totalPaid;

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

            ReservationStageApproved = confirmedReservations,
            ReservationStagePending = awaitingPaymentReservations,
            PaymentStageApproved = confirmedReservations,
            PaymentStagePending = awaitingPaymentReservations,
            BookingStageApproved = confirmedReservations,
            BookingStagePending = awaitingPaymentReservations
        };

        //var reservations = await _context.Reservations
        //    .Include(r => r.Club)
        //    .Include(r => r.ReservationRooms)
        //    .Include(r => r.PaymentIntents)
        //        .ThenInclude(pi => pi.Payments)
        //    .ToListAsync(ct);

        //var reservationStatuses = reservations.Select(r =>
        //{
        //    decimal paid = r.PaymentIntents?
        //        .SelectMany(pi => pi.Payments)
        //        .Where(p => p.Status == PaymentStatus.Paid)
        //        .Sum(p => p.Amount) ?? 0;

        //    return new
        //    {
        //        Reservation = r,
        //        Paid = paid,
        //        DisplayStatus = GetDisplayStatus(r, paid)
        //    };
        //}).ToList();

        //int totalReservations = reservationStatuses.Count;
        //int confirmedReservations = reservationStatuses.Count(x => x.DisplayStatus == "Booking Confirmed");
        //int cancelledReservations = reservationStatuses.Count(x => x.DisplayStatus == "Cancelled");
        //int awaitingPaymentReservations = reservationStatuses.Count(x => x.DisplayStatus == "Awaiting Payment");

        //// Active = confirmed + awaiting payment
        //int activeReservations = confirmedReservations + awaitingPaymentReservations;

        //// ✅ Payments & Revenue
        //decimal totalPaid = reservationStatuses.Sum(x => x.Paid);
        //decimal totalRevenue = totalPaid;

        //// ✅ Club-wise stats
        //var clubStats = reservationStatuses
        //    .GroupBy(x => x.Reservation.Club?.Name ?? "Unknown Club")
        //    .Select(g => new ClubDashboardDto
        //    {
        //        ClubName = g.Key,
        //        TotalReservations = g.Count(),
        //        ConfirmedReservations = g.Count(r => r.DisplayStatus == "Booking Confirmed"),
        //        CancelledReservations = g.Count(r => r.DisplayStatus == "Cancelled"),
        //        AwaitingPaymentReservations = g.Count(r => r.DisplayStatus == "Awaiting Payment"),
        //        TotalRoomsBooked = g.Where(r => r.DisplayStatus != "Cancelled")
        //                            .SelectMany(r => r.Reservation.ReservationRooms).Count(),
        //        ConfirmationPercentage = g.Any()
        //            ? Math.Round((decimal)g.Count(r => r.DisplayStatus == "Booking Confirmed") / g.Count() * 100, 2)
        //            : 0
        //    })
        //    .ToList();

        //// ✅ Stage progress (Approved + Pending + Cancelled = TotalReservations)
        //int reservationStageApproved = confirmedReservations;
        //int reservationStagePending = awaitingPaymentReservations;

        //// Cancelled is tracked separately
        //int paymentStageApproved = confirmedReservations; // paid fully
        //int paymentStagePending = awaitingPaymentReservations; // awaiting only

        //int bookingStageApproved = confirmedReservations;
        //int bookingStagePending = awaitingPaymentReservations;

        //return new ReservationDashboardDto
        //{
        //    TotalReservations = totalReservations,
        //    ActiveReservations = activeReservations,
        //    CancelledReservations = cancelledReservations,
        //    CompletedReservations = confirmedReservations,
        //    TotalRevenue = totalRevenue,
        //    TotalPaid = totalPaid,

        //    AwaitingPaymentReservations = awaitingPaymentReservations,
        //    Clubs = clubStats,

        //    ReservationStageApproved = reservationStageApproved,
        //    ReservationStagePending = reservationStagePending,
        //    PaymentStageApproved = paymentStageApproved,
        //    PaymentStagePending = paymentStagePending,
        //    BookingStageApproved = bookingStageApproved,
        //    BookingStagePending = bookingStagePending
        //};
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

