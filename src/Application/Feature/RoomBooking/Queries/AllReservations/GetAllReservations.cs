using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Commands.CreateReservation.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Reservations.Dtos;
using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.ReservationStatus.Dtos;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.AllReservations;
public class GetAllReservationsuery() : IRequest<List<ReservationWebDto>>;
public class GetAllReservationsueryHandler : IRequestHandler<GetAllReservationsuery, List<ReservationWebDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOLMRSApplicationDbContext _context;

    public GetAllReservationsueryHandler(IOLMRSApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<ReservationWebDto>> Handle(GetAllReservationsuery request, CancellationToken ct)
    {
        // 1️⃣ Get all reservations with details
        var reservations = await _context.Reservations
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
            .ToListAsync(ct);

        // 2️⃣ Collect all userIds
        var userIds = reservations.Select(r => r.UserId.ToString()).Distinct().ToList();

        // 3️⃣ Fetch users from Identity
        var usersDict = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                PhoneNumber = u.MobileNo ?? u.MobileNo
            })
            .ToDictionaryAsync(x => x.Id, x => new UserInfoDto
            {
                UserId = Guid.Parse(x.Id),
                Name = x.UserName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            }, ct);

        // 4️⃣ Map into DTOs
        var result = reservations.Select(r =>
        {
            decimal paid = r.PaymentIntents
                .SelectMany(pi => pi.Payments)
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount);

            string paymentStatus = paid >= r.TotalAmount
                ? "Paid"
                : paid > 0 ? "Partially Paid" : "Pending";

            string displayStatus = GetDisplayStatus(r, paid);

            return new ReservationWebDto
            {
                ReservationId = r.Id,
                OneBillId = r.OneBillId,
                Status = displayStatus,
                PaymentStatus = paymentStatus,

                Created = r.Created,
                CreatedBy = r.CreatedBy,

                RoomsAmount = r.RoomsAmount,
                Taxes = r.Taxes,
                Discounts = r.Discounts,
                TotalAmount = r.TotalAmount,
                AmountPaid = paid,

                User = usersDict.ContainsKey(r.UserId.ToString())
                    ? usersDict[r.UserId.ToString()]
                    : new UserInfoDto { UserId = r.UserId, Name = "Unknown" },

                Club = new ClubInfoDto
                {
                    Name = r.Club.Name,
                    Location = r.Club.Location,
                    ContactNumber = r.Club.ContactNumber
                },

                Guest = r.Guest == null ? null : new CreateGuestDto
                {
                    FullName = r.Guest.FullName,
                    CNICOrPassport = r.Guest.CNICOrPassport,
                    Phone = r.Guest.Phone,
                    Email = r.Guest.Email
                },

                Rooms = r.ReservationRooms.Select(rr => new ReservationRoomDetailDto
                {
                    RoomId = rr.Room.Id,
                    RoomNo = rr.Room?.No ?? string.Empty,
                    RoomName = rr.Room?.Name ?? string.Empty,
                    CategoryName = rr.Room?.RoomCategory?.Name ?? string.Empty,
                    ResidenceType = rr.Room?.ResidenceType?.Name ?? string.Empty,
                    Services = rr.Room?.RoomServices?.Select(s => s.Name).ToList() ?? new List<string>(),

                    FromDate = rr.FromDate,
                    ToDate = rr.ToDate,
                    TotalNights = (rr.ToDate.Date - rr.FromDate.Date).Days,
                    PricePerNight = rr.PricePerNight,
                    Subtotal = rr.Subtotal
                }).ToList(),

                PaymentIntents = r.PaymentIntents.Select(pi => new PaymentIntentDto
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
                }).ToList()
            };
        }).OrderByDescending(x => x.Created).ToList();

        return result;
    }


    private string GetDisplayStatus(Reservation r, decimal paid)
    {
        if (paid >= r.TotalAmount)
            return "Booking Confirmed";

        if (r.ExpiresAt <= DateTime.Now)
            return "Cancelled"; // or "Expired"

        return "Awaiting Payment";
    }
}
