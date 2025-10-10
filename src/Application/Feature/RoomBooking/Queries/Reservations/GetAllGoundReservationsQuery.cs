using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations.Dtos;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
public record GetAllGroundReservationsQuery(Guid UserId) : IRequest<List<ReservationCardDto>>;

public class GetAllGroundReservationsQueryHandler : IRequestHandler<GetAllGroundReservationsQuery, List<ReservationCardDto>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetAllGroundReservationsQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReservationCardDto>> Handle(GetAllGroundReservationsQuery request, CancellationToken ct)
    {
        var reservations = await _context.Reservations
            .AsNoTracking()
            .Include(r => r.Club)
            .Include(r => r.ReservationRooms)
            .Include(r => r.PaymentIntents)
                .ThenInclude(pi => pi.Payments)
            .Where(r => r.UserId == request.UserId
             && r.Club.ClubType == ClubType.Ground) // 👈 filter by type
             .ToListAsync(ct);

        var result = reservations.Select(r =>
        {
            decimal paid = r.PaymentIntents
                .SelectMany(pi => pi.Payments)
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount);

            string displayStatus = GetDisplayStatus(r, paid);

            string paymentStatus;
            if (paid >= r.TotalAmount)
                paymentStatus = "Paid";
            else if (paid > 0)
                paymentStatus = "Partially Paid";
            else
                paymentStatus = "Pending";

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
        }).ToList();


        return result
    .OrderByDescending(x => x.CreatedDateTime)
    .ToList();

    }

    private string GetDisplayStatus(Reservation r, decimal paid)
    {
        if (paid >= r.TotalAmount)
            return "Booking Confirmed";

        if (r.ExpiresAt <= DateTime.Now)
            return "Expired"; // or "Cancelled"

        return "Awaiting Payment";
    }



    //public async Task<List<ReservationListDto>> Handle(GetAllReservationsQuery request, CancellationToken cancellationToken)
    //{
    //    var reservations = await _context.Reservations
    //        .Where(r => r.UserId == request.UserId)
    //        .Include(r => r.ReservationRooms)
    //            .ThenInclude(rr => rr.Room)
    //                .ThenInclude(room => room.ResidenceType)
    //        .Include(r => r.ReservationRooms)
    //            .ThenInclude(rr => rr.Room)
    //                .ThenInclude(room => room.RoomCategory)
    //        .Include(r => r.Club)
    //        .ToListAsync(cancellationToken);

    //    var result = reservations
    //        .Select(r => new ReservationListDto
    //        {
    //            ReservationId = r.Id,
    //            TotalAmount = r.TotalAmount,
    //            Rooms = r.ReservationRooms.Select(rr => new ReservationRoomListDto
    //            {
    //                RoomNo = rr.Room?.No ?? string.Empty,
    //                ClubName = r.Club?.Name ?? string.Empty,
    //                ResidenceType = rr.Room?.ResidenceType?.Name ?? string.Empty,
    //                RoomCategory = rr.Room?.RoomCategory?.Name ?? string.Empty,
    //                FromDate = rr.FromDate,
    //                ToDate = rr.ToDate
    //            }).ToList()
    //        })
    //        .ToList();

    //    return result;
    //}
}
