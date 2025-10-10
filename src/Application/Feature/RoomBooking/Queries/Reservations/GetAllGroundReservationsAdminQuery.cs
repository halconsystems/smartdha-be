using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations.Dtos;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
public record GetAllGroundReservationsAdminQuery() : IRequest<List<ReservationCardDto>>;

public class GetAllGroundReservationsAdminQueryHandler
    : IRequestHandler<GetAllGroundReservationsAdminQuery, List<ReservationCardDto>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetAllGroundReservationsAdminQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReservationCardDto>> Handle(GetAllGroundReservationsAdminQuery request, CancellationToken ct)
    {
        var reservations = await _context.Reservations
            .AsNoTracking()
            .Include(r => r.Club)
            .Include(r => r.ReservationRooms)
            .Include(r => r.PaymentIntents).ThenInclude(pi => pi.Payments)
            .Where(r => r.Club.ClubType == ClubType.Ground) // 👈 only filter by Ground
            .ToListAsync(ct);

        var result = reservations.Select(r =>
        {
            decimal paid = r.PaymentIntents
                .SelectMany(pi => pi.Payments)
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount);

            string displayStatus = GetDisplayStatus(r, paid);

            string paymentStatus = paid >= r.TotalAmount
                ? "Paid"
                : paid > 0
                    ? "Partially Paid"
                    : "Pending";

            return new ReservationCardDto
            {
                ReservationId = r.Id,
                OneBillId = r.OneBillId,
                ClubName = r.Club.Name,
                ClubLocation = r.Club.Location ?? string.Empty,
                FromDate = r.ReservationRooms.Any() ? r.ReservationRooms.Min(rr => rr.FromDate) : DateTime.MinValue,
                ToDate = r.ReservationRooms.Any() ? r.ReservationRooms.Max(rr => rr.ToDate) : DateTime.MinValue,
                Nights = r.ReservationRooms.Any()
                    ? (r.ReservationRooms.Max(rr => rr.ToDate).Date - r.ReservationRooms.Min(rr => rr.FromDate).Date).Days
                    : 0,
                RoomsCount = r.ReservationRooms.Count,
                TotalAmount = r.TotalAmount,
                AmountPaid = paid,
                PaymentStatus = paymentStatus,
                Status = displayStatus,
                CreatedDateTime = r.Created
            };
        }).OrderByDescending(x => x.CreatedDateTime)
          .ToList();

        return result;
    }

    private string GetDisplayStatus(Reservation r, decimal paid)
    {
        if (paid >= r.TotalAmount)
            return "Booking Confirmed";

        if (r.ExpiresAt <= DateTime.Now)
            return "Expired"; // or "Cancelled"

        return "Awaiting Payment";
    }
}

